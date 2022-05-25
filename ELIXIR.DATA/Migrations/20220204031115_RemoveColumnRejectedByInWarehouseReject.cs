using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveColumnRejectedByInWarehouseReject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "Warehouse_Reject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectedBy",
                table: "Warehouse_Reject",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
