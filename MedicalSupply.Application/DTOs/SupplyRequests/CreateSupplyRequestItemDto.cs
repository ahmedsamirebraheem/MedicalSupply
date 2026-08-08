using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class CreateSupplyRequestItemDto
    {
        public int ItemId { get; set; }
        public int RequestedQuantity { get; set; }
    }
}
