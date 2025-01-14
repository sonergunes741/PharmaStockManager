using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Painkillers" },
                    { 2, "Antibiotics" },
                    { 3, "Vitamins" },
                    { 4, "Cough Syrups" },
                    { 5, "Diabetes Medications" },
                    { 6, "Antihistamines" }
                });

            migrationBuilder.InsertData(
                table: "Drugs",
                columns: new[] { "Id", "Category", "CriticalStockLevel", "DrugType", "ExpiryDate", "MaxRequest", "Name", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Painkillers", 10, "Tablet", null, 50, "Paracetamol", 100, 5.99m },
                    { 2, "Antibiotics", 10, "Capsule", null, 50, "Amoxicillin", 50, 12.50m },
                    { 3, "Vitamins", 10, "Syrup", null, 50, "Vitamin C", 200, 8.99m },
                    { 4, "Painkillers", 10, "Tablet", null, 50, "Ibuprofen", 150, 6.50m },
                    { 5, "Antibiotics", 10, "Capsule", null, 50, "Azithromycin", 70, 10.00m },
                    { 6, "Cough Syrups", 10, "Syrup", null, 50, "Dextromethorphan", 120, 7.99m },
                    { 7, "Diabetes Medications", 10, "Tablet", null, 50, "Metformin", 80, 15.50m },
                    { 8, "Antihistamines", 10, "Tablet", null, 50, "Cetirizine", 90, 4.99m }
                });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Id", "DrugId", "IsApproved", "IsRejected", "Quantity", "RequestDate", "UserName" },
                values: new object[,]
                {
                    { 1, 1, true, false, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1" },
                    { 2, 2, false, true, 5, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2" },
                    { 3, 3, false, false, 20, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "user3" },
                    { 4, 4, true, false, 8, new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "user4" },
                    { 5, 5, false, false, 12, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "user5" },
                    { 6, 6, true, false, 15, new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "user6" },
                    { 7, 7, false, true, 6, new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "user7" },
                    { 8, 8, false, false, 25, new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user8" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
