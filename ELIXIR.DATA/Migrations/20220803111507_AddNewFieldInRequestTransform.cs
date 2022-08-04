using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewFieldInRequestTransform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "Transformation_Request",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "Transformation_Request");
        }
    }
}
