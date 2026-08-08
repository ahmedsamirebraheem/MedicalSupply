using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class SupplyRequestDetailsDto
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? RejectionReason { get; set; }

        public DepartmentSummaryDto Department { get; set; } = null!;
        public List<SupplyRequestItemDto> Items { get; set; } = new();
        public List<ApprovalRecordDto> ApprovalRecords { get; set; } = new();
    }
}
