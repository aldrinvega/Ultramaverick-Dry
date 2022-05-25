using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewColumnInPlanningRemarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelRemarks",
                table: "Transformation_Planning",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectRemarks",
                table: "Transformation_Planning",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelRemarks",
                table: "Transformation_Planning");

            migrationBuilder.DropColumn(
                name: "RejectRemarks",
                table: "Transformation_Planning");
        }
    }
}
