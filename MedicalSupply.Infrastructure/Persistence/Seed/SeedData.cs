using MedicalSupply.Domain.Entities;
using MedicalSupply.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task SeedAsync(MedicalSupplyDbContext context)
        {
            if (await context.Departments.AnyAsync())
                return; 


            var departments = new List<Department>
            {
                new Department("PHARM", "Pharmacy", 50000m),
                new Department("CLINIC", "Clinics", 30000m),
                new Department("NURSING", "Nursing Units", 25000m),
                new Department("LAB", "Laboratories", 20000m),
                new Department("FINANCE", "Finance", 15000m),
                new Department("ADMIN", "Administration", 10000m)
            };

            await context.Departments.AddRangeAsync(departments);

            var items = new List<Item>
            {
                new Item("MED-001", "Paracetamol 500mg", ItemCategory.Medication, 5.50m, 1000,
                    requiresPharmacyApproval: true, isControlledMedication: false),

                new Item("MED-002", "Morphine 10mg", ItemCategory.Medication, 45.00m, 100,
                    requiresPharmacyApproval: true, isControlledMedication: true),

                new Item("SUP-001", "Surgical Gloves (Box)", ItemCategory.MedicalSupply, 12.00m, 500,
                    requiresPharmacyApproval: false, isControlledMedication: false),

                new Item("SUP-002", "IV Cannula", ItemCategory.MedicalSupply, 3.25m, 800,
                    requiresPharmacyApproval: false, isControlledMedication: false),

                new Item("LAB-001", "Blood Collection Tubes (Pack of 100)", ItemCategory.LaboratorySupply, 18.00m, 300,
                    requiresPharmacyApproval: false, isControlledMedication: false),

                new Item("OFF-001", "A4 Paper (Ream)", ItemCategory.OfficeSupply, 4.00m, 200,
                    requiresPharmacyApproval: false, isControlledMedication: false)
            };

            await context.Items.AddRangeAsync(items);

            await context.SaveChangesAsync();
        }
    }
}
