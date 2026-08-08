# Medical Supply Request Management System

A backend API for managing medical and administrative supply requests across hospital departments (Pharmacy, Clinics, Nursing, Laboratories, Finance, Administration), built with ASP.NET Core Web API following Clean Architecture.

## 1. Project Overview

The system allows departments to submit supply requests containing one or more inventory items. Each request goes through a dynamic approval flow (Department Manager → Pharmacy if applicable → Finance if applicable) before stock is reserved and eventually fulfilled from inventory.

## 2. Required Software

- .NET 8+ (developed and tested on .NET 10)
- SQL Server (LocalDB, Express, or full instance)
- A REST client (Postman recommended) or the built-in Swagger UI

## 3. Run Instructions

```bash
git clone <repository-url>
cd MedicalSupply

dotnet restore

# Update connection string in src/MedicalSupply.Api/appsettings.json if needed

dotnet ef database update --project src/MedicalSupply.Infrastructure --startup-project src/MedicalSupply.Api

dotnet run --project src/MedicalSupply.Api
```

Navigate to `https://localhost:<port>/swagger` to access the Swagger UI. Seed data (departments and items) is inserted automatically on first run.

## 4. Database Configuration

Connection string is located in `src/MedicalSupply.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MedicalSupplyDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Update `Server=.` to match your local SQL Server instance name (e.g. `.\SQLEXPRESS`).

## 5. Migration Commands

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project src/MedicalSupply.Infrastructure --startup-project src/MedicalSupply.Api --output-dir Persistence/Migrations

# Apply migrations to the database
dotnet ef database update --project src/MedicalSupply.Infrastructure --startup-project src/MedicalSupply.Api
```

Applied migrations:
1. `InitialCreate` — core schema (Departments, Items, SupplyRequests, SupplyRequestItems, ApprovalRecords)
2. `FixRowVersionType` — corrected RowVersion column type for optimistic concurrency
3. `AddRequestNumberSequence` — SQL Server SEQUENCE for unique request number generation

## 6. Authentication Instructions & Sample Users

The API uses JWT Bearer Authentication. Users are hardcoded (per assessment scope — no user registration system) in `MedicalSupply.Infrastructure/Identity/HardcodedUserStore.cs`.

**Login endpoint:** `POST /api/auth/login`

```json
{
  "email": "admin@company.com",
  "password": "Pass123!"
}
```

All demo users share the password `Pass123!`:

| Email | Role |
|---|---|
| requester@company.com | Requester |
| manager@company.com | DepartmentManager |
| pharmacist@company.com | Pharmacist |
| finance@company.com | FinanceOfficer |
| storekeeper@company.com | StoreKeeper |
| admin@company.com | Administrator |

Copy the returned `token` into Swagger's **Authorize** button (or the `Authorization: Bearer <token>` header in Postman) to access protected endpoints.

## 7. Clean Architecture Explanation & Dependency Direction

The solution is split into four projects:

```
MedicalSupply.Domain          (no dependencies)
MedicalSupply.Application     → depends on Domain only
MedicalSupply.Infrastructure  → depends on Application + Domain
MedicalSupply.Api             → depends on Application + Infrastructure
```

- **Domain** holds entities (`Department`, `Item`, `SupplyRequest`, `SupplyRequestItem`, `ApprovalRecord`), enums, and domain exceptions. Entities protect their own invariants (private setters, constructor validation, behavior-driven methods like `Approve()`, `Reserve()`, `Fulfill()`). No EF Core, no HTTP, no database access.
- **Application** contains the use cases (`SupplyRequestService`, `DepartmentService`, `ItemService`), DTOs, and abstractions (`IApplicationDbContext`, `ICurrentUserService`, `IRequestNumberGenerator`, `ITokenGenerator`) that Infrastructure implements. Repository/UnitOfWork patterns were intentionally omitted — `IApplicationDbContext` plus EF Core's built-in `SaveChangesAsync` already provide sufficient abstraction and atomicity for this project's scope (see Section 12).
- **Infrastructure** implements persistence (EF Core `DbContext`, entity configurations, migrations, seed data), JWT token generation, and the hardcoded user store.
- **Api** exposes HTTP endpoints, wires up JWT authentication/authorization, Swagger, and centralized exception handling. Controllers are thin — they call into Application services and translate results to HTTP responses; they never touch `DbContext` directly.

CQRS/MediatR were intentionally not used — the project's size doesn't justify the added indirection (per assessment guidance that these patterns should only be used when they add clear value).

## 8. Main Business Rules

- A request must contain ≥1 item, each requested quantity > 0, no duplicate items per request.
- Unit price is captured on the request item at creation time (price snapshot).
- Request number is unique, generated via a SQL Server `SEQUENCE` (format `SR-{year}-{6-digit sequence}`).
- Approval flow is determined dynamically: Department Manager is always required; Pharmacy is required if any item requires pharmacy approval or is a controlled medication; Finance is required if the total exceeds 10,000.
- Submission is rejected if the total exceeds the department's remaining monthly budget (monthly budget minus the sum of all non-draft, non-rejected, non-cancelled requests for the current month).
- Stock is validated and reserved only after all required approvals are complete — never at creation or submission time.
- Cancellation releases reserved stock (if the request was Approved) atomically with the status change.
- Fulfillment reduces both reserved and available quantity and can only happen once (enforced by the status check itself).

## 9. Transaction Boundaries

Each use case in `SupplyRequestService` follows the same pattern: all entity mutations (on `SupplyRequest` and related `Item` entities) happen in memory first, and `SaveChangesAsync()` is called exactly once at the end of the method. EF Core wraps all pending changes from a single `SaveChangesAsync()` call in one implicit database transaction — if any part fails (including a concurrency conflict), everything rolls back together. No explicit `BeginTransaction()` is needed because a single `DbContext` instance is used throughout each request.

## 10. Concurrency Handling Approach

- **Inventory concurrency:** `Item` and `SupplyRequest` both have a `RowVersion` shadow property (SQL Server `rowversion` column) configured via `IsRowVersion()`. If two operations try to update the same row concurrently, EF Core throws `DbUpdateConcurrencyException` on the second `SaveChangesAsync()` call, which the global exception middleware translates to `409 Conflict`.
- **Request number uniqueness:** generated via a SQL Server `SEQUENCE`, which guarantees uniqueness under concurrent access via an internal atomic increment. Note: `SEQUENCE` can produce gaps if a transaction using a generated value is rolled back — this is an accepted trade-off since the requirement is uniqueness, not gap-free sequencing.
- **Budget check race condition:** not fully protected against two simultaneous submissions both passing the budget check before either commits (known limitation, see Section 13).

## 11. How Partial Database Updates Are Prevented

Same mechanism as Section 9 — all related changes (request status, approval records, item reservations) are accumulated in memory and persisted through a single `SaveChangesAsync()` call, which EF Core executes as one atomic transaction. A failure at any point (validation, concurrency conflict) prevents any partial write.

## 12. Assumptions

- `Code` fields (Department, Item) are capped at 20 characters; `Name` fields at 200 characters — these limits are not specified in the assessment and were chosen as reasonable defaults.
- A unique index was added on `Department.Code` and `Item.Code` even though not explicitly requested, to prevent ambiguous lookups (the assessment explicitly requires "appropriate indexes and constraints" generally).
- "Monthly budget consumed" is calculated from all requests with status `PendingManagerApproval` through `Fulfilled` in the current calendar month (Draft, Rejected, and Cancelled requests are excluded as they don't represent a real commitment against the budget).
- Items cannot be added to a request once it leaves Draft status (not explicit in the assessment, but inferred to protect data integrity of an in-progress or completed approval flow).
- `UpdateItemDto` allows updating Code, Name, Category, UnitPrice, RequiresPharmacyApproval, and IsControlledMedication. `AvailableQuantity` is deliberately excluded — this field is explicitly stated in the assessment to be modified only through Reserve/Release/Fulfill operations, never through a direct edit.
- `IApplicationDbContext` exposes `IQueryable<T>` rather than `DbSet<T>` to keep the Application layer decoupled from EF Core-specific types, while `Microsoft.EntityFrameworkCore` (the core package only, no provider) is referenced in Application solely to use async LINQ operators (`FirstOrDefaultAsync`, `ToListAsync`, etc.) — no SQL Server-specific code exists outside Infrastructure.

## 13. Known Limitations

- Hardcoded users store passwords in plain text and compares them with a direct string comparison — acceptable only because the assessment explicitly scopes user management out (demonstration purposes only). A real system would hash passwords (e.g. BCrypt) and use a real user store.
- The JWT secret key is committed in `appsettings.json` for demo convenience. In production this must live in a secret manager / environment variable, never in source control.
- The monthly budget check (Section 10) has a narrow race-condition window between reading the remaining budget and committing the submission — two simultaneous submissions near the budget limit could both pass validation. Mitigating this fully would require a stricter isolation level or a budget-tracking row with its own concurrency token, which was out of scope given the 24-hour time-box.
- No refresh tokens, rate limiting, or API versioning implemented (all listed as optional in the assessment).
- No automated tests (explicitly excluded from assessment scope).

## 14. Technical Decisions & Trade-offs

- **No Repository/UnitOfWork pattern**: `IApplicationDbContext` + EF Core's native `SaveChangesAsync` already provide persistence abstraction and atomicity; an additional Repository layer would add indirection without solving an unaddressed problem.
- **No CQRS/MediatR**: the project's use case count and team size don't justify the added ceremony.
- **Enums stored as strings** (not integers) in the database for human-readable data when inspected directly in SQL Server Management Studio, at a small storage cost.
- **SQL Server SEQUENCE over a manually-managed counter table** for request numbers — SEQUENCE has built-in atomic increment behavior; a manual table would require explicit row locking to achieve the same safety.

## 15. Unfinished Requirements

None of the mandatory scope items (Section 15 of the assessment) were left unfinished. Optional items (notifications/message queue, Docker, CI/CD, API versioning, rate limiting, distributed caching, refresh tokens, health checks) were not implemented, per the assessment's own prioritization rule that optional items should not be pursued at the expense of the mandatory business flow.

## 16. Additional Features Implemented

None beyond the mandatory and explicitly suggested scope (e.g. unique indexes beyond the one explicitly required for request numbers, as documented in Section 12).
