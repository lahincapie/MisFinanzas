using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisFinanzas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOneOffToExpenseAndIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOneOff",
                table: "Incomes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOneOff",
                table: "Expenses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOneOff",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "IsOneOff",
                table: "Expenses");
        }
    }
}
