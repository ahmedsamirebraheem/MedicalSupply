using MedicalSupply.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.Items
{
    public class CreateItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ItemCategory Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public bool RequiresPharmacyApproval { get; set; }
        public bool IsControlledMedication { get; set; }
    }
}
