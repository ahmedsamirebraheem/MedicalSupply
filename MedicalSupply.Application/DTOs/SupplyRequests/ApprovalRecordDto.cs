using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class ApprovalRecordDto
    {
        public string ApprovalType { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string DecisionBy { get; set; } = string.Empty;
        public DateTime DecisionDate { get; set; }
        public string? Comments { get; set; }
    }
}
