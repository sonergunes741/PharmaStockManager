using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id");
        }
    }
}
