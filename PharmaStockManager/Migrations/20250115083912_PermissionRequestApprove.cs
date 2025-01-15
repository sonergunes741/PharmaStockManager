using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaStockManager.Migrations
{
    /// <inheritdoc />
    public partial class PermissionRequestApprove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequestApprove",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestApprove",
                table: "Permissions");
        }
    }
}
