using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class AddParacetamol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Drugs",
                columns: new[] { "Id", "Category", "ExpiryDate", "Name", "Quantity", "UnitPrice" },
                values: new object[] { 3, "Painkillers", null, "Paracetamol", 100, 8.0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
