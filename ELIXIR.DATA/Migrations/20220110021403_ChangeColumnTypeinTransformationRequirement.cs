using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class ChangeColumnTypeinTransformationRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormulaRequirements_RawMaterials_RawMaterialId",
                table: "FormulaRequirements");

            migrationBuilder.DropColumn(
                name: "ItemCodeId",
                table: "FormulaRequirements");

            migrationBuilder.AlterColumn<int>(
                name: "RawMaterialId",
                table: "FormulaRequirements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormulaRequirements_RawMaterials_RawMaterialId",
                table: "FormulaRequirements",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormulaRequirements_RawMaterials_RawMaterialId",
                table: "FormulaRequirements");

            migrationBuilder.AlterColumn<int>(
                name: "RawMaterialId",
                table: "FormulaRequirements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ItemCodeId",
                table: "FormulaRequirements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormulaRequirements_RawMaterials_RawMaterialId",
                table: "FormulaRequirements",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
