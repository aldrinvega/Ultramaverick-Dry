using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableTranformationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transformation_Planning_Transformation_Request_RequestId",
                table: "Transformation_Planning");

            migrationBuilder.DropIndex(
                name: "IX_Transformation_Planning_RequestId",
                table: "Transformation_Planning");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Transformation_Planning");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Transformation_Planning",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transformation_Planning_RequestId",
                table: "Transformation_Planning",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transformation_Planning_Transformation_Request_RequestId",
                table: "Transformation_Planning",
                column: "RequestId",
                principalTable: "Transformation_Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
