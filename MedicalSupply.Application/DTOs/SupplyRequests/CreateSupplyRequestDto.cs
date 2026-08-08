using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class CreateSupplyRequestDto
    {
        public int DepartmentId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public List<CreateSupplyRequestItemDto> Items { get; set; } = new();
    }
}
