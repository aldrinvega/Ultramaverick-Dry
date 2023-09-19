using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddModifiedandAddedByinChecklistTypesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.AddColumn<int>(
                name: "AddedBy",
                table: "ChecklistTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ChecklistTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ChecklistTypeId",
                table: "ChecklistQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_AddedBy",
                table: "ChecklistTypes",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTypes_ModifiedBy",
                table: "ChecklistTypes",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId",
                principalTable: "ChecklistTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_Users_AddedBy",
                table: "ChecklistTypes",
                column: "AddedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistTypes_Users_ModifiedBy",
                table: "ChecklistTypes",
                column: "ModifiedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_Users_AddedBy",
                table: "ChecklistTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistTypes_Users_ModifiedBy",
                table: "ChecklistTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistTypes_AddedBy",
                table: "ChecklistTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistTypes_ModifiedBy",
                table: "ChecklistTypes");

            migrationBuilder.DropColumn(
                name: "AddedBy",
                table: "ChecklistTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ChecklistTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ChecklistTypeId",
                table: "ChecklistQuestions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistQuestions_ChecklistTypes_ChecklistTypeId",
                table: "ChecklistQuestions",
                column: "ChecklistTypeId",
                principalTable: "ChecklistTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
