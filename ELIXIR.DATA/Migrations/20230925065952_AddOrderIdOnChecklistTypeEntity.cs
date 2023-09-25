using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddOrderIdOnChecklistTypeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "QcChecklists",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ChecklistTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ChecklistTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "QcChecklists",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QcChecklists_ProductTypes_ProductTypeId",
                table: "QcChecklists",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
