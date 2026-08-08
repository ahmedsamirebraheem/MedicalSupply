using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class DepartmentSummaryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
