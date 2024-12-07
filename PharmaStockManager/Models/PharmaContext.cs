using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public DbSet<Depos> Depos { get; set; } //Depo Dbset
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
                new Drug { Id = 1, Name = "Aspirin", Category = "Painkillers", Quantity = 50, UnitPrice = 10.0m },
                new Drug { Id = 2, Name = "Amoxicillin", Category = "Antibiotics", Quantity = 30, UnitPrice = 20.0m },
                new Drug { Id = 3, Name = "Paracetamol", Category = "Painkillers", Quantity = 100, UnitPrice = 8.0m }
                );
            modelBuilder.Entity<Depos>().HasData(
                new Depos { Id = 1, Name = "Depo A"},
                new Depos { Id = 2 ,Name = "Depo B"}
                );
        }
    }
}
