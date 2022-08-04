using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTransformationField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Transformation_Request",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Transformation_Request");
        }
    }
}
