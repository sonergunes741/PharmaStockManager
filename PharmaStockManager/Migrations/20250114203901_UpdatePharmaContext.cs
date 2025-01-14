using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePharmaContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Painkillers" },
                    { 2, "Antibiotics" }
                });

            migrationBuilder.InsertData(
                table: "Drugs",
                columns: new[] { "Id", "Category", "CriticalStockLevel", "DrugType", "ExpiryDate", "MaxRequest", "Name", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Painkillers", 10, "Commercial", null, 50, "Aspirin", 50, 10.0m },
                    { 2, "Antibiotics", 10, "Commercial", null, 50, "Amoxicillin", 30, 20.0m },
                    { 3, "Painkillers", 10, "Commercial", null, 50, "Paracetamol", 100, 8.0m }
                });
        }
    }
}
