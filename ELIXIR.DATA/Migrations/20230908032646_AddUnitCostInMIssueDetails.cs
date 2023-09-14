using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddUnitCostInMIssueDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "MiscellaneousIssueDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "MiscellaneousIssueDetails");
        }
    }
}
