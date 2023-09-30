using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatGPTtrading.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addfakestat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FakeStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalInvestorsCount = table.Column<int>(type: "integer", nullable: false),
                    TodayInvestorsCount = table.Column<int>(type: "integer", nullable: false),
                    NeededInvestorsCount = table.Column<int>(type: "integer", nullable: false),
                    TotalWithdrawalsAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TodayWithdrawalsAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NeededWithdrawalsAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MinWithdrawalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxWithdrawalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TodayDatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FakeStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FakeStats");
        }
    }
}
