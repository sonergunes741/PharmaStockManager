using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class DrugType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 1,
                column: "DrugType",
                value: "Klinik");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 2,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 3,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 4,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 5,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 6,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 7,
                column: "DrugType",
                value: "Ticari");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 8,
                column: "DrugType",
                value: "Ticari");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 1,
                column: "DrugType",
                value: "Tablet");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 2,
                column: "DrugType",
                value: "Capsule");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 3,
                column: "DrugType",
                value: "Syrup");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 4,
                column: "DrugType",
                value: "Tablet");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 5,
                column: "DrugType",
                value: "Capsule");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 6,
                column: "DrugType",
                value: "Syrup");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 7,
                column: "DrugType",
                value: "Tablet");

            migrationBuilder.UpdateData(
                table: "Drugs",
                keyColumn: "Id",
                keyValue: 8,
                column: "DrugType",
                value: "Tablet");
        }
    }
}
