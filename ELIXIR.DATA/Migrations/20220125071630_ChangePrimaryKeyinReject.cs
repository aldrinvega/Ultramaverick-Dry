using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class ChangePrimaryKeyinReject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject");

            migrationBuilder.AlterColumn<int>(
                name: "PO_ReceivingId",
                table: "QC_Reject",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject");

            migrationBuilder.AlterColumn<int>(
                name: "PO_ReceivingId",
                table: "QC_Reject",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_QC_Reject_QC_Receiving_PO_ReceivingId",
                table: "QC_Reject",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
