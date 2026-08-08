using MedicalSupply.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        IQueryable<Department> Departments { get; }
        IQueryable<Item> Items { get; }
        IQueryable<SupplyRequest> SupplyRequests { get; }
        IQueryable<SupplyRequestItem> SupplyRequestItems { get; }
        IQueryable<ApprovalRecord> ApprovalRecords { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void AddSupplyRequest(SupplyRequest request);
        void AddDepartment(Department department);
        void AddItem(Item item);
    }
}
