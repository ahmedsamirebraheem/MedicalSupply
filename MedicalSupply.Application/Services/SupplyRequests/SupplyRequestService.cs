using MedicalSupply.Application.Abstractions.Persistence;
using MedicalSupply.Application.Abstractions.Services;
using MedicalSupply.Application.Common;
using MedicalSupply.Application.DTOs.SupplyRequests;
using MedicalSupply.Application.Exceptions;
using MedicalSupply.Domain.Entities;
using MedicalSupply.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.Services.SupplyRequests
{
    public class SupplyRequestService
    {
        private readonly IApplicationDbContext _context;
        private readonly IRequestNumberGenerator _requestNumberGenerator;

        public SupplyRequestService(
            IApplicationDbContext context,
            IRequestNumberGenerator requestNumberGenerator)
        {
            _context = context;
            _requestNumberGenerator = requestNumberGenerator;
        }

        public async Task<int> CreateDraftRequestAsync(
                CreateSupplyRequestDto dto,
                 CancellationToken cancellationToken = default)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId, cancellationToken);

            if (department is null)
                throw new NotFoundException($"Department with id {dto.DepartmentId} was not found.");

            if (!department.IsActive)
                throw new BusinessRuleException("The selected department is not active.");

            if (!dto.Items.Any())
                throw new BusinessRuleException("A request must contain at least one item.");

            var itemIds = dto.Items.Select(i => i.ItemId).ToList();

            var items = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            var request = new SupplyRequest(dto.DepartmentId, dto.RequestedBy);

            foreach (var itemDto in dto.Items)
            {
                var item = items.FirstOrDefault(i => i.Id == itemDto.ItemId);

                if (item is null)
                    throw new NotFoundException($"Item with id {itemDto.ItemId} was not found.");

                if (!item.IsActive)
                    throw new BusinessRuleException($"Item '{item.Name}' is not active.");

                request.AddItem(itemDto.ItemId, itemDto.RequestedQuantity, item.UnitPrice);
            }

            var requestNumber = await _requestNumberGenerator.GenerateAsync(cancellationToken);
            request.SetRequestNumber(requestNumber);

            _context.AddSupplyRequest(request);
            await _context.SaveChangesAsync(cancellationToken);

            return request.Id;
        }

        public async Task SubmitRequestAsync(int requestId, CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

            if (department is null)
                throw new NotFoundException($"Department with id {request.DepartmentId} was not found.");

            var consumedThisMonth = await GetConsumedBudgetForCurrentMonthAsync(
                request.DepartmentId, cancellationToken);

            var remainingBudget = department.MonthlyBudget - consumedThisMonth;

            request.Submit(remainingBudget);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<decimal> GetConsumedBudgetForCurrentMonthAsync(
            int departmentId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var consumedStatuses = new[]
            {
        RequestStatus.PendingManagerApproval,
        RequestStatus.PendingPharmacyApproval,
        RequestStatus.PendingFinanceApproval,
        RequestStatus.Approved,
        RequestStatus.Fulfilled
    };

            return await _context.SupplyRequests
                .Where(r => r.DepartmentId == departmentId
                         && r.RequestDate >= startOfMonth
                         && consumedStatuses.Contains(r.Status))
                .SumAsync(r => r.TotalAmount, cancellationToken);
        }

        public async Task ApproveRequestAsync(
    int requestId,
    ApprovalType approvalType,
    string decisionBy,
    string? comments,
    CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.Items)
                .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            request.Approve(approvalType, decisionBy, comments);

            if (request.Status == RequestStatus.Approved)
            {
                var itemIds = request.Items.Select(i => i.ItemId).ToList();

                var items = await _context.Items
                    .Where(i => itemIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                foreach (var requestItem in request.Items)
                {
                    var item = items.First(i => i.Id == requestItem.ItemId);
                    item.Reserve(requestItem.RequestedQuantity);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RejectRequestAsync(
    int requestId,
    ApprovalType approvalType,
    string decisionBy,
    string reason,
    CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            request.Reject(approvalType, decisionBy, reason);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CancelRequestAsync(int requestId, CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            var wasApproved = request.Cancel();

            if (wasApproved)
            {
                var itemIds = request.Items.Select(i => i.ItemId).ToList();

                var items = await _context.Items
                    .Where(i => itemIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                foreach (var requestItem in request.Items)
                {
                    var item = items.First(i => i.Id == requestItem.ItemId);
                    item.ReleaseReservation(requestItem.RequestedQuantity);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task FulfillRequestAsync(int requestId, CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            request.Fulfill();

            var itemIds = request.Items.Select(i => i.ItemId).ToList();

            var items = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            foreach (var requestItem in request.Items)
            {
                var item = items.First(i => i.Id == requestItem.ItemId);
                item.Fulfill(requestItem.RequestedQuantity);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<SupplyRequestDetailsDto> GetRequestDetailsAsync(
    int requestId, CancellationToken cancellationToken = default)
        {
            var request = await _context.SupplyRequests
                .Include(r => r.Department)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Item)
                .Include(r => r.ApprovalRecords)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request is null)
                throw new NotFoundException($"Supply request with id {requestId} was not found.");

            return MapToDetailsDto(request);
        }

        private static SupplyRequestDetailsDto MapToDetailsDto(SupplyRequest request)
        {
            return new SupplyRequestDetailsDto
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                RequestedBy = request.RequestedBy,
                RequestDate = request.RequestDate,
                Status = request.Status.ToString(),
                TotalAmount = request.TotalAmount,
                RejectionReason = request.RejectionReason,

                Department = new DepartmentSummaryDto
                {
                    Id = request.Department.Id,
                    Code = request.Department.Code,
                    Name = request.Department.Name
                },

                Items = request.Items.Select(i => new SupplyRequestItemDto
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    ItemName = i.Item.Name,
                    RequestedQuantity = i.RequestedQuantity,
                    ApprovedQuantity = i.ApprovedQuantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),

                ApprovalRecords = request.ApprovalRecords.Select(a => new ApprovalRecordDto
                {
                    ApprovalType = a.ApprovalType.ToString(),
                    Decision = a.Decision.ToString(),
                    DecisionBy = a.DecisionBy,
                    DecisionDate = a.DecisionDate,
                    Comments = a.Comments
                }).ToList()
            };
        }

        public async Task<PagedResult<SupplyRequestSummaryDto>> SearchSupplyRequestsAsync(
    SearchSupplyRequestsQuery query, CancellationToken cancellationToken = default)
        {
            var requestsQuery = _context.SupplyRequests
                .Include(r => r.Department)
                .AsQueryable();

            if (query.DepartmentId.HasValue)
                requestsQuery = requestsQuery.Where(r => r.DepartmentId == query.DepartmentId.Value);

            if (query.Status.HasValue)
                requestsQuery = requestsQuery.Where(r => r.Status == query.Status.Value);

            if (query.FromDate.HasValue)
                requestsQuery = requestsQuery.Where(r => r.RequestDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                requestsQuery = requestsQuery.Where(r => r.RequestDate <= query.ToDate.Value);

            var totalCount = await requestsQuery.CountAsync(cancellationToken);

            var items = await requestsQuery
                .OrderByDescending(r => r.RequestDate)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new SupplyRequestSummaryDto
                {
                    Id = r.Id,
                    RequestNumber = r.RequestNumber,
                    DepartmentName = r.Department.Name,
                    RequestedBy = r.RequestedBy,
                    RequestDate = r.RequestDate,
                    Status = r.Status.ToString(),
                    TotalAmount = r.TotalAmount
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<SupplyRequestSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
