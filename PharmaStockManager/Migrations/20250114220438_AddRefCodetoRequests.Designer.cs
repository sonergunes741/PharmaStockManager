﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PharmaStockManager.Models;

#nullable disable

namespace PharmaStockManager.Migrations
{
    [DbContext(typeof(PharmaContext))]
    [Migration("20250114220438_AddRefCodetoRequests")]
    partial class AddRefCodetoRequests
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence<int>("UserIds")
                .StartsAt(1000L);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PharmaStockManager.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Painkillers"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Antibiotics"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Vitamins"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Cough Syrups"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Diabetes Medications"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Antihistamines"
                        });
                });

            modelBuilder.Entity("PharmaStockManager.Models.Drug", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CriticalStockLevel")
                        .HasColumnType("int");

                    b.Property<string>("DrugType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MaxRequest")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Drugs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Category = "Painkillers",
                            CriticalStockLevel = 10,
                            DrugType = "Tablet",
                            MaxRequest = 50,
                            Name = "Paracetamol",
                            Quantity = 100,
                            RefCode = "ADMIN001",
                            UnitPrice = 5.99m
                        },
                        new
                        {
                            Id = 2,
                            Category = "Antibiotics",
                            CriticalStockLevel = 10,
                            DrugType = "Capsule",
                            MaxRequest = 50,
                            Name = "Amoxicillin",
                            Quantity = 50,
                            RefCode = "ADMIN001",
                            UnitPrice = 12.50m
                        },
                        new
                        {
                            Id = 3,
                            Category = "Vitamins",
                            CriticalStockLevel = 10,
                            DrugType = "Syrup",
                            MaxRequest = 50,
                            Name = "Vitamin C",
                            Quantity = 200,
                            RefCode = "ADMIN001",
                            UnitPrice = 8.99m
                        },
                        new
                        {
                            Id = 4,
                            Category = "Painkillers",
                            CriticalStockLevel = 10,
                            DrugType = "Tablet",
                            MaxRequest = 50,
                            Name = "Ibuprofen",
                            Quantity = 150,
                            RefCode = "ADMIN001",
                            UnitPrice = 6.50m
                        },
                        new
                        {
                            Id = 5,
                            Category = "Antibiotics",
                            CriticalStockLevel = 10,
                            DrugType = "Capsule",
                            MaxRequest = 50,
                            Name = "Azithromycin",
                            Quantity = 70,
                            RefCode = "ADMIN001",
                            UnitPrice = 10.00m
                        },
                        new
                        {
                            Id = 6,
                            Category = "Cough Syrups",
                            CriticalStockLevel = 10,
                            DrugType = "Syrup",
                            MaxRequest = 50,
                            Name = "Dextromethorphan",
                            Quantity = 120,
                            RefCode = "ADMIN001",
                            UnitPrice = 7.99m
                        },
                        new
                        {
                            Id = 7,
                            Category = "Diabetes Medications",
                            CriticalStockLevel = 10,
                            DrugType = "Tablet",
                            MaxRequest = 50,
                            Name = "Metformin",
                            Quantity = 80,
                            RefCode = "ADMIN001",
                            UnitPrice = 15.50m
                        },
                        new
                        {
                            Id = 8,
                            Category = "Antihistamines",
                            CriticalStockLevel = 10,
                            DrugType = "Tablet",
                            MaxRequest = 50,
                            Name = "Cetirizine",
                            Quantity = 90,
                            RefCode = "ADMIN001",
                            UnitPrice = 4.99m
                        });
                });

            modelBuilder.Entity("PharmaStockManager.Models.Identity.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("PharmaStockManager.Models.Identity.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("NEXT VALUE FOR UserIds");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int?>("ActivationCode")
                        .HasColumnType("int");

                    b.Property<bool>("ActiveUser")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RefCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("UserType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("PharmaStockManager.Models.Permissions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("EditStocks")
                        .HasColumnType("bit");

                    b.Property<bool>("StockIn")
                        .HasColumnType("bit");

                    b.Property<bool>("StockOut")
                        .HasColumnType("bit");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserID");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("PharmaStockManager.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DrugId")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRejected")
                        .HasColumnType("bit");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Requests");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DrugId = 1,
                            IsApproved = true,
                            IsRejected = false,
                            Quantity = 10,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user1"
                        },
                        new
                        {
                            Id = 2,
                            DrugId = 2,
                            IsApproved = false,
                            IsRejected = true,
                            Quantity = 5,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user2"
                        },
                        new
                        {
                            Id = 3,
                            DrugId = 3,
                            IsApproved = false,
                            IsRejected = false,
                            Quantity = 20,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user3"
                        },
                        new
                        {
                            Id = 4,
                            DrugId = 4,
                            IsApproved = true,
                            IsRejected = false,
                            Quantity = 8,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user4"
                        },
                        new
                        {
                            Id = 5,
                            DrugId = 5,
                            IsApproved = false,
                            IsRejected = false,
                            Quantity = 12,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user5"
                        },
                        new
                        {
                            Id = 6,
                            DrugId = 6,
                            IsApproved = true,
                            IsRejected = false,
                            Quantity = 15,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user6"
                        },
                        new
                        {
                            Id = 7,
                            DrugId = 7,
                            IsApproved = false,
                            IsRejected = true,
                            Quantity = 6,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user7"
                        },
                        new
                        {
                            Id = 8,
                            DrugId = 8,
                            IsApproved = false,
                            IsRejected = false,
                            Quantity = 25,
                            RefCode = "ADMIN001",
                            RequestDate = new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserName = "user8"
                        });
                });

            modelBuilder.Entity("PharmaStockManager.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DrugName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("PharmaStockManager.Models.Warehouse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RefCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("RefCode")
                        .IsUnique();

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("PharmaStockManager.Services.LogFilter+LogClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PharmaStockManager.Models.Permissions", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PharmaStockManager.Services.LogFilter+LogClass", b =>
                {
                    b.HasOne("PharmaStockManager.Models.Identity.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PharmaStockManager.Models.Identity.AppUser", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
