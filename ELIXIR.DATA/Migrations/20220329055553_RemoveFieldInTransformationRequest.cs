using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveFieldInTransformationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawmaterialQuantity",
                table: "Transformation_Request");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RawmaterialQuantity",
                table: "Transformation_Request",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
