using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class transactionchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_userId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Drugs_DrugId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DrugId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DrugId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Logs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_userId",
                table: "Logs",
                newName: "IX_Logs_UserId");

            migrationBuilder.AddColumn<string>(
                name: "DrugName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "DrugName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Logs",
                newName: "createdAt");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                newName: "IX_Logs_userId");

            migrationBuilder.AddColumn<int>(
                name: "DrugId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DrugId",
                table: "Transactions",
                column: "DrugId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_userId",
                table: "Logs",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Drugs_DrugId",
                table: "Transactions",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
