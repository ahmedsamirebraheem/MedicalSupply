using MedicalSupply.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Entities
{
    public class ApprovalRecord
    {
        public int Id { get; private set; }
        public int SupplyRequestId { get; private set; }
        public ApprovalType ApprovalType { get; private set; }
        public ApprovalDecision Decision { get; private set; }
        public string DecisionBy { get; private set; } = null!;
        public DateTime DecisionDate { get; private set; }
        public string? Comments { get; private set; }

        private ApprovalRecord() { }

        public ApprovalRecord(ApprovalType approvalType, ApprovalDecision decision, string decisionBy, string? comments)
        {
            if (string.IsNullOrWhiteSpace(decisionBy))
                throw new ArgumentException("DecisionBy is required.");

            ApprovalType = approvalType;
            Decision = decision;
            DecisionBy = decisionBy;
            Comments = comments;
            DecisionDate = DateTime.UtcNow;
        }
    }
}
