using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisFinanzas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Incomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");
        }
    }
}
