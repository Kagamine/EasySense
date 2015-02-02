using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace EasySense.Models
{
    public class EasySenseContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<EnterpriseModel> Enterprises { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<ZoneModel> Zones { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<ReportModel> Reports { get; set; }
        public DbSet<AlarmModel> Alarms { get; set; }

        public EasySenseContext() : base("mssqldb") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>()
                .HasMany(x => x.Projects)
                .WithOptional(x => x.User);

            modelBuilder.Entity<UserModel>()
                .HasMany(x => x.Reports)
                .WithRequired(x => x.User);

            modelBuilder.Entity<UserModel>()
                .HasMany(x => x.Files)
                .WithRequired(x=>x.User);

            modelBuilder.Entity<UserModel>()
                .HasMany(x => x.Alarms)
                .WithRequired(x => x.User);

            modelBuilder.Entity<CategoryModel>()
                .HasMany(x => x.Products)
                .WithRequired(x => x.Category);

            modelBuilder.Entity<EnterpriseModel>()
                .HasMany(x => x.Customers)
                .WithRequired(x => x.Enterprise);

            modelBuilder.Entity<DepartmentModel>()
                .HasMany(x => x.Users)
                .WithOptional(x => x.Department);
        }
    }
}