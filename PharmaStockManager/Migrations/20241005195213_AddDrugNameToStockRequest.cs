using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class AddDrugNameToStockRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DrugName",
                table: "StockRequests",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_DrugId",
                table: "StockRequests",
                column: "DrugId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests");

            migrationBuilder.DropIndex(
                name: "IX_StockRequests_DrugId",
                table: "StockRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StockRequests",
                newName: "DrugName");
        }
    }
}
