using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Entities
{
    public class SupplyRequestItem
    {
        public int Id { get; private set; }
        public int SupplyRequestId { get; private set; }
        public int ItemId { get; private set; }
        public int RequestedQuantity { get; private set; }
        public int? ApprovedQuantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }

        public Item Item { get; private set; } = null!;
        private SupplyRequestItem()
        {
            
        }
        public SupplyRequestItem(int itemId, int requestedQuantity, decimal unitPrice)
        {
            if (requestedQuantity <= 0)
                throw new ArgumentException("Requested quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            ItemId = itemId;
            RequestedQuantity = requestedQuantity;
            UnitPrice = unitPrice;
            TotalPrice = requestedQuantity * unitPrice;
        }
    }
}
