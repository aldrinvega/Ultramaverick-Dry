using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustChecklistFKandRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ChecklistQuestions",
                newName: "ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistQuestions_ProductTypeId",
                table: "ChecklistQuestions",
                newName: "IX_ChecklistQuestions_ModifiedBy");

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
                name: "FK_ChecklistQuestions_Users_ModifiedBy",
                table: "ChecklistQuestions",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_Users_ModifiedBy",
                table: "ChecklistQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "ChecklistQuestions",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ChecklistQuestions_ModifiedBy",
                table: "ChecklistQuestions",
                newName: "IX_ChecklistQuestions_ProductTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductTypeId",
                table: "ChecklistTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ProductTypes_ProductTypeId",
                table: "ChecklistQuestions",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_ProductTypes_ProductTypeId",
                table: "ChecklistTypes",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
