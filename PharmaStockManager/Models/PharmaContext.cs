using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PharmaStockManager.Models
{
    public class PharmaContext : IdentityDbContext<IdentityUser>
    {
        public PharmaContext(DbContextOptions<PharmaContext> options) : base(options)
        {
        }

        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; } // Yeni eklenen Transaction DbSet'i

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Painkillers" },
                new Category { Id = 2, Name = "Antibiotics" }
            );

            // Seed data for Drugs
            modelBuilder.Entity<Drug>().HasData(
                new Drug
                {
                    Id = 1,
                    Name = "Aspirin",
                    DrugType = "Commercial",
                    ResearchNumber = "R12345",
                    SendLocation = "Warehouse A",
                    NameSurname = "John Doe",
                    TelephoneNumber = "1234567890",
                    Email = "john.doe@example.com",
                    Address = "123 Main St",
                    Category = "Painkillers",
                    Quantity = 50,
                    UnitPrice = 10.0m
                },
                new Drug
                {
                    Id = 2,
                    Name = "Amoxicillin",
                    DrugType = "Clinical",
                    ResearchNumber = "RN67890",
                    SendLocation = "Warehouse B",
                    NameSurname = "Jane Smith",
                    TelephoneNumber = "0987654321",
                    Email = "jane.smith@example.com",
                    Address = "456 Elm St",
                    Category = "Antibiotics",
                    Quantity = 30,
                    UnitPrice = 20.0m
                }
            );
        }
    }
}