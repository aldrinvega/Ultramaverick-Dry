using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIR.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddjustRelationshipfromuserModuletoModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_RoleModules_ModuleId",
                table: "RoleModules");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_Modules_ModuleId",
                table: "RoleModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_Modules_ModuleId",
                table: "RoleModules");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_RoleModules_ModuleId",
                table: "RoleModules",
                column: "ModuleId",
                principalTable: "RoleModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
