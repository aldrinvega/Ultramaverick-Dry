using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddReceivingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.AddColumn<string>(
                name: "ProductType",
                table: "QC_Receiving",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PO_ReceivingId",
                table: "CheckListStrings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ReceivingId",
                table: "CheckListStrings",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "ReceivingId",
                table: "CheckListStrings");

            migrationBuilder.AlterColumn<int>(
                name: "PO_ReceivingId",
                table: "CheckListStrings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
