using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class NullableProductIdonChecklistTypeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "ChecklistTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "ChecklistTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
