using MedicalSupply.Domain.Enums;
using MedicalSupply.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Entities
{
    public class Item
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public ItemCategory Category { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        public bool RequiresPharmacyApproval { get; private set; }
        public bool IsControlledMedication { get; private set; }
        public bool IsActive { get; private set; }
        private Item()
        {
            
        }
        public Item(
        string code,
        string name,
        ItemCategory category,
        decimal unitPrice,
        int availableQuantity,
        bool requiresPharmacyApproval,
        bool isControlledMedication)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Item code is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Item name is required.");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            if (availableQuantity < 0)
                throw new ArgumentException("Available quantity cannot be negative.");

            Code = code;
            Name = name;
            Category = category;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
            ReservedQuantity = 0;
            RequiresPharmacyApproval = requiresPharmacyApproval;
            IsControlledMedication = isControlledMedication;
            IsActive = true;
        }

        public int GetUnreservedQuantity() => AvailableQuantity - ReservedQuantity;

        public void Reserve(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Reserved quantity must be greater than zero.");

            if (quantity > GetUnreservedQuantity())
                throw new InsufficientStockException("Insufficient stock to reserve the requested quantity.");

            ReservedQuantity += quantity;
        }

        public void ReleaseReservation(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Released quantity must be greater than zero.");

            ReservedQuantity -= quantity;
        }

        public void Fulfill(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Fulfilled quantity must be greater than zero.");

            ReservedQuantity -= quantity;
            AvailableQuantity -= quantity;
        }
        public void UpdateDetails(
    string code, string name, ItemCategory category, decimal unitPrice,
    bool requiresPharmacyApproval, bool isControlledMedication)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Item code is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Item name is required.");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Code = code;
            Name = name;
            Category = category;
            UnitPrice = unitPrice;
            RequiresPharmacyApproval = requiresPharmacyApproval;
            IsControlledMedication = isControlledMedication;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
