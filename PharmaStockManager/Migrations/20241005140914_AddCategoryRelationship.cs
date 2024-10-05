using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Drugs");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Drugs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_CategoryId",
                table: "Drugs",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drugs_Categories_CategoryId",
                table: "Drugs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drugs_Categories_CategoryId",
                table: "Drugs");

            migrationBuilder.DropIndex(
                name: "IX_Drugs_CategoryId",
                table: "Drugs");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Drugs");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Drugs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
