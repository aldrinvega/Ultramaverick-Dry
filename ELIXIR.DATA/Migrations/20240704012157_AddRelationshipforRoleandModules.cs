using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIR.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipforRoleandModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoleModules_ModuleId",
                table: "RoleModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleModules_RoleId",
                table: "RoleModules",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_RoleModules_ModuleId",
                table: "RoleModules",
                column: "ModuleId",
                principalTable: "RoleModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_Roles_RoleId",
                table: "RoleModules",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_RoleModules_ModuleId",
                table: "RoleModules");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_Roles_RoleId",
                table: "RoleModules");

            migrationBuilder.DropIndex(
                name: "IX_RoleModules_ModuleId",
                table: "RoleModules");

            migrationBuilder.DropIndex(
                name: "IX_RoleModules_RoleId",
                table: "RoleModules");
        }
    }
}
