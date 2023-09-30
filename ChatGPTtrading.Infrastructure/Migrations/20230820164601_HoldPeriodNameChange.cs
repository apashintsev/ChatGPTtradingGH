using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatGPTtrading.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoldPeriodNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoldPeriodInMonths",
                table: "PlatformSettings",
                newName: "HoldPeriodInMinutes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoldPeriodInMinutes",
                table: "PlatformSettings",
                newName: "HoldPeriodInMonths");
        }
    }
}
