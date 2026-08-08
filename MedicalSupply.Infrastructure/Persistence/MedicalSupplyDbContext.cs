using MedicalSupply.Application.Abstractions.Persistence;
using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence
{
    public class MedicalSupplyDbContext : DbContext, IApplicationDbContext
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

        IQueryable<Department> IApplicationDbContext.Departments => Departments;

        IQueryable<Item> IApplicationDbContext.Items => Items;

        IQueryable<SupplyRequest> IApplicationDbContext.SupplyRequests => SupplyRequests;

        IQueryable<SupplyRequestItem> IApplicationDbContext.SupplyRequestItems => SupplyRequestItems;

        IQueryable<ApprovalRecord> IApplicationDbContext.ApprovalRecords => ApprovalRecords;

        public void AddDepartment(Department department) => Departments.Add(department);

        public void AddItem(Item item) => Items.Add(item);

        public void AddSupplyRequest(SupplyRequest request) => SupplyRequests.Add(request);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicalSupplyDbContext).Assembly);

            modelBuilder.HasSequence<int>("RequestNumberSequence")
                .StartsAt(1)
                .IncrementsBy(1);

            base.OnModelCreating(modelBuilder);
        }
    }
}
