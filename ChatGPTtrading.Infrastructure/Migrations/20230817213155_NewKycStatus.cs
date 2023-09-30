using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatGPTtrading.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewKycStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KycSended",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KycSended",
                table: "Users");
        }
    }
}
