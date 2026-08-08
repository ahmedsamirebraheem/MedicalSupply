using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.Items
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public bool RequiresPharmacyApproval { get; set; }
        public bool IsControlledMedication { get; set; }
        public bool IsActive { get; set; }
    }
}
