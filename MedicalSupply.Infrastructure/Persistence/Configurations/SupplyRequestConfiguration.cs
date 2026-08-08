using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence.Configurations
{
    public class SupplyRequestConfiguration : IEntityTypeConfiguration<SupplyRequest>
    {
        public void Configure(EntityTypeBuilder<SupplyRequest> builder)
        {
            builder.Property(r => r.RequestNumber)
              .IsRequired()
              .HasMaxLength(30);

            builder.HasIndex(r => r.RequestNumber)
                .IsUnique();

            builder.Property(r => r.RequestedBy)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(r => r.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.RejectionReason)
                .HasMaxLength(500);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasOne(r => r.Department)
                .WithMany()
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Items)
                .WithOne()
                .HasForeignKey(i => i.SupplyRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.ApprovalRecords)
                .WithOne()
                .HasForeignKey(a => a.SupplyRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.DepartmentId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.RequestDate);

            builder.Property<byte[]>("RowVersion")
                .IsRowVersion();
        }


    }
}
