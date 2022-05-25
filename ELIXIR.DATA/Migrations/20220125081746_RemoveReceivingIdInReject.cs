using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class RemoveReceivingIdInReject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject");

            migrationBuilder.DropIndex(
                name: "IX_QC_Reject_PO_ReceivingId",
                table: "QC_Reject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QC_Reject_PO_ReceivingId",
                table: "QC_Reject",
                column: "PO_ReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
