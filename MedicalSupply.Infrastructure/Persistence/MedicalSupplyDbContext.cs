using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence
{
    public class MedicalSupplyDbContext: DbContext
    {
        public MedicalSupplyDbContext(DbContextOptions<MedicalSupplyDbContext> options)
           : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SupplyRequest> SupplyRequests { get; set; }
        public DbSet<SupplyRequestItem> SupplyRequestItems { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicalSupplyDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
