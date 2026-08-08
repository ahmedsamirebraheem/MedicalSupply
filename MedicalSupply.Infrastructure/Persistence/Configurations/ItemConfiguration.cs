using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Property(i => i.Code)
              .IsRequired()
              .HasMaxLength(20);

            builder.HasIndex(i => i.Code)
                .IsUnique();

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.AvailableQuantity)
                .IsRequired();

            builder.Property(i => i.ReservedQuantity)
                .IsRequired();

            builder.Property(i => i.RequiresPharmacyApproval)
                .IsRequired();

            builder.Property(i => i.IsControlledMedication)
                .IsRequired();

            builder.Property(i => i.IsActive)
                .IsRequired();

            builder.Property<byte[]>("RowVersion")
                .IsRowVersion();
        }

        
    }
}
