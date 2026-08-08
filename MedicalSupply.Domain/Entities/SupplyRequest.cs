using MedicalSupply.Domain.Enums;
using MedicalSupply.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedicalSupply.Domain.Entities
{
    public class SupplyRequest
    {
        public int Id { get; private set; }
        public string RequestNumber { get; private set; } = null!;
        public int DepartmentId { get; private set; }
        public string RequestedBy { get; private set; } = null!;
        public DateTime RequestDate { get; private set; }
        public RequestStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string? RejectionReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public Department Department { get; private set; } = null!;

        private readonly List<SupplyRequestItem> _items = [];
        private readonly List<ApprovalRecord> _approvalRecords = [];

        public IReadOnlyCollection<SupplyRequestItem> Items => _items.AsReadOnly();
        public IReadOnlyCollection<ApprovalRecord> ApprovalRecords => _approvalRecords.AsReadOnly();

        private SupplyRequest()
        {
            
        }
        public SupplyRequest(int departmentId, string requestedBy)
        {
            if (string.IsNullOrWhiteSpace(requestedBy))
                throw new ArgumentException("RequestedBy is required.");

            DepartmentId = departmentId;
            RequestedBy = requestedBy;
            RequestDate = DateTime.UtcNow;
            Status = RequestStatus.Draft;
            CreatedAt = DateTime.UtcNow;
            TotalAmount = 0;
        }

        public void SetRequestNumber(string requestNumber)
        {
            if (string.IsNullOrWhiteSpace(requestNumber))
                throw new ArgumentException("Request number is required.");

            RequestNumber = requestNumber;
        }

        public void AddItem(int itemId, int requestedQuantity, decimal unitPrice)
        {
            if (Status != RequestStatus.Draft)
                throw new InvalidStatusTransitionException("Items can only be added while the request is in Draft status.");

            if (_items.Any(i => i.ItemId == itemId))
                throw new InvalidOperationException("This item has already been added to the request.");

            var requestItem = new SupplyRequestItem(itemId, requestedQuantity, unitPrice);
            _items.Add(requestItem);

            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal()
        {
            TotalAmount = _items.Sum(i => i.TotalPrice);
        }

        public void Submit(decimal departmentRemainingBudget)
        {
            if (Status != RequestStatus.Draft)
                throw new InvalidStatusTransitionException("Only a request in Draft status can be submitted.");

            if (!_items.Any())
                throw new InvalidOperationException("A request must contain at least one item.");

            if (TotalAmount > departmentRemainingBudget)
                throw new BudgetExceededException("The total request amount exceeds the department's remaining monthly budget.");

            Status = DetermineNextApprovalStatus();
            UpdatedAt = DateTime.UtcNow;
        }

        private RequestStatus DetermineNextApprovalStatus()
        {
            return RequestStatus.PendingManagerApproval;
        }

        public void Approve(ApprovalType approvalType, string decisionBy, string? comments)
        {
            var expectedStatus = GetPendingStatusFor(approvalType);

            if (Status != expectedStatus)
                throw new InvalidStatusTransitionException($"This request is not currently waiting for {approvalType} approval.");

            if (_approvalRecords.Any(a => a.ApprovalType == approvalType))
                throw new DuplicateApprovalException($"{approvalType} approval has already been completed for this request.");

            var record = new ApprovalRecord(approvalType, ApprovalDecision.Approved, decisionBy, comments);
            _approvalRecords.Add(record);

            Status = DetermineNextStatusAfter(approvalType);
            UpdatedAt = DateTime.UtcNow;
        }

        private RequestStatus GetPendingStatusFor(ApprovalType approvalType) => approvalType switch
        {
            ApprovalType.DepartmentManager => RequestStatus.PendingManagerApproval,
            ApprovalType.Pharmacy => RequestStatus.PendingPharmacyApproval,
            ApprovalType.Finance => RequestStatus.PendingFinanceApproval,
            _ => throw new ArgumentOutOfRangeException(nameof(approvalType))
        };

        private RequestStatus DetermineNextStatusAfter(ApprovalType completedType)
        {
            if (completedType == ApprovalType.DepartmentManager)
            {
                if (RequiresPharmacyApproval()) return RequestStatus.PendingPharmacyApproval;
                if (RequiresFinanceApproval()) return RequestStatus.PendingFinanceApproval;
                return RequestStatus.Approved;
            }

            if (completedType == ApprovalType.Pharmacy)
            {
                if (RequiresFinanceApproval()) return RequestStatus.PendingFinanceApproval;
                return RequestStatus.Approved;
            }

            return RequestStatus.Approved;
        }

        private bool RequiresPharmacyApproval() =>
            _items.Any(i => i.Item.RequiresPharmacyApproval || i.Item.IsControlledMedication);

        private bool RequiresFinanceApproval() =>
            TotalAmount > 10000m;

        public void Reject(ApprovalType approvalType, string decisionBy, string reason)
        {
            var expectedStatus = GetPendingStatusFor(approvalType);

            if (Status != expectedStatus)
                throw new InvalidStatusTransitionException($"This request is not currently waiting for {approvalType} approval.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("A rejection reason is required.");

            var record = new ApprovalRecord(approvalType, ApprovalDecision.Rejected, decisionBy, reason);
            _approvalRecords.Add(record);

            Status = RequestStatus.Rejected;
            RejectionReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }
        public bool Cancel()
        {
            if (Status == RequestStatus.Fulfilled)
                throw new InvalidStatusTransitionException("A fulfilled request cannot be cancelled.");

            if (Status == RequestStatus.Rejected)
                throw new InvalidStatusTransitionException("A rejected request cannot be cancelled.");

            if (Status == RequestStatus.Cancelled)
                throw new InvalidStatusTransitionException("This request has already been cancelled.");

            var wasApproved = Status == RequestStatus.Approved;

            Status = RequestStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;

            return wasApproved;
        }
        public void Fulfill()
        {
            if (Status != RequestStatus.Approved)
                throw new InvalidStatusTransitionException("Only an approved request can be fulfilled.");

            Status = RequestStatus.Fulfilled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
