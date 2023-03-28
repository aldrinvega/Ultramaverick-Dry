using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistForCompliant_QC_Receiving_PO_ReceivingId",
                table: "ChecklistForCompliant");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckListInput_QC_Receiving_PO_ReceivingId",
                table: "CheckListInput");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.DropIndex(
                name: "IX_CheckListStrings_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.DropIndex(
                name: "IX_CheckListInput_PO_ReceivingId",
                table: "CheckListInput");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistForCompliant_PO_ReceivingId",
                table: "ChecklistForCompliant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CheckListStrings_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListInput_PO_ReceivingId",
                table: "CheckListInput",
                column: "PO_ReceivingId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistForCompliant_PO_ReceivingId",
                table: "ChecklistForCompliant",
                column: "PO_ReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistForCompliant_QC_Receiving_PO_ReceivingId",
                table: "ChecklistForCompliant",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListInput_QC_Receiving_PO_ReceivingId",
                table: "CheckListInput",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
