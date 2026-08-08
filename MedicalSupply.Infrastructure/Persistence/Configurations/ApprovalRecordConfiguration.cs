using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence.Configurations
{
    public class ApprovalRecordConfiguration : IEntityTypeConfiguration<ApprovalRecord>
    {
        public void Configure(EntityTypeBuilder<ApprovalRecord> builder)
        {

            builder.Property(a => a.ApprovalType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(a => a.Decision)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(a => a.DecisionBy)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.DecisionDate)
                .IsRequired();

            builder.Property(a => a.Comments)
                .HasMaxLength(500);
        }
    }
}
