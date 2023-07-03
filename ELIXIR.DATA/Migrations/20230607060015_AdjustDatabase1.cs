using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustDatabase1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.DropIndex(
                name: "IX_CheckListStrings_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.DropColumn(
                name: "PO_ReceivingId",
                table: "CheckListStrings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PO_ReceivingId",
                table: "CheckListStrings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckListStrings_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
