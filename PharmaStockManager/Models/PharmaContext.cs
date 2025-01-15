using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static PharmaStockManager.Services.LogFilter;

namespace PharmaStockManager.Models
{
    public class PharmaContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public PharmaContext(DbContextOptions<PharmaContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer();
        }

        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; } // Yeni eklenen Transaction DbSet'i
        public DbSet<Request> Requests { get; set; } //Requests DbSet

        public DbSet<LogClass> Logs { get; set; }

        public DbSet<Permissions> Permissions { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the IdentityUser (or AppUser) to start from 1000
            modelBuilder.Entity<AppUser>()
                .HasKey(u => u.Id);

            // If you're using SQL Server, you can use this approach
            modelBuilder.HasSequence<int>("UserIds")
                .StartsAt(1000)
                .IncrementsBy(1);

            modelBuilder.Entity<AppUser>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEXT VALUE FOR UserIds");


            // Categories Dummy Data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Painkillers" },
                new Category { Id = 2, Name = "Antibiotics" },
                new Category { Id = 3, Name = "Vitamins" },
                new Category { Id = 4, Name = "Cough Syrups" },
                new Category { Id = 5, Name = "Diabetes Medications" },
                new Category { Id = 6, Name = "Antihistamines" }
            );

            // Drugs Dummy Data
            modelBuilder.Entity<Drug>().HasData(
                new Drug { Id = 1, Name = "Paracetamol", Category = "Painkillers", Quantity = 100, UnitPrice = 5.99m, DrugType = "Klinik", RefCode = "ADMIN001" },
                new Drug { Id = 2, Name = "Amoxicillin", Category = "Antibiotics", Quantity = 50, UnitPrice = 12.50m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 3, Name = "Vitamin C", Category = "Vitamins", Quantity = 200, UnitPrice = 8.99m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 4, Name = "Ibuprofen", Category = "Painkillers", Quantity = 150, UnitPrice = 6.50m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 5, Name = "Azithromycin", Category = "Antibiotics", Quantity = 70, UnitPrice = 10.00m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 6, Name = "Dextromethorphan", Category = "Cough Syrups", Quantity = 120, UnitPrice = 7.99m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 7, Name = "Metformin", Category = "Diabetes Medications", Quantity = 80, UnitPrice = 15.50m, DrugType = "Ticari", RefCode = "ADMIN001" },
                new Drug { Id = 8, Name = "Cetirizine", Category = "Antihistamines", Quantity = 90, UnitPrice = 4.99m, DrugType = "Ticari", RefCode = "ADMIN001" }
            );


            // Requests Dummy Data
            modelBuilder.Entity<Request>().HasData(
                new Request { Id = 1, UserName = "user1", DrugId = 1, Quantity = 10, RequestDate = new DateTime(2025, 1, 1), IsApproved = true, IsRejected = false },
                new Request { Id = 2, UserName = "user2", DrugId = 2, Quantity = 5, RequestDate = new DateTime(2025, 1, 2), IsApproved = false, IsRejected = true },
                new Request { Id = 3, UserName = "user3", DrugId = 3, Quantity = 20, RequestDate = new DateTime(2025, 1, 3), IsApproved = false, IsRejected = false },
                new Request { Id = 4, UserName = "user4", DrugId = 4, Quantity = 8, RequestDate = new DateTime(2025, 1, 4), IsApproved = true, IsRejected = false },
                new Request { Id = 5, UserName = "user5", DrugId = 5, Quantity = 12, RequestDate = new DateTime(2025, 1, 5), IsApproved = false, IsRejected = false },
                new Request { Id = 6, UserName = "user6", DrugId = 6, Quantity = 15, RequestDate = new DateTime(2025, 1, 6), IsApproved = true, IsRejected = false },
                new Request { Id = 7, UserName = "user7", DrugId = 7, Quantity = 6, RequestDate = new DateTime(2025, 1, 7), IsApproved = false, IsRejected = true },
                new Request { Id = 8, UserName = "user8", DrugId = 8, Quantity = 25, RequestDate = new DateTime(2025, 1, 8), IsApproved = false, IsRejected = false }
            );

            // Warehouses Dummy Data
            modelBuilder.Entity<Warehouse>().HasIndex(w => w.RefCode).IsUnique();
        }
    }
}
