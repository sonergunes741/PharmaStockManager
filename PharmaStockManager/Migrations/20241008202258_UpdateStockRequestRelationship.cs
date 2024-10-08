using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStockRequestRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Drugs_DrugId1",
                table: "StockRequests");

            migrationBuilder.DropIndex(
                name: "IX_StockRequests_DrugId1",
                table: "StockRequests");

            migrationBuilder.DropColumn(
                name: "DrugId1",
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

            migrationBuilder.AddColumn<int>(
                name: "DrugId1",
                table: "StockRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_DrugId1",
                table: "StockRequests",
                column: "DrugId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Drugs_DrugId",
                table: "StockRequests",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Drugs_DrugId1",
                table: "StockRequests",
                column: "DrugId1",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
