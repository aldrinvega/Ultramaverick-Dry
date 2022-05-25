using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewColumnInQcReceiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "ExpiryIsApprove",
                table: "QC_Receiving",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "ExpiryApproveBy",
                table: "QC_Receiving",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDateOfApprove",
                table: "QC_Receiving",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNearlyExpire",
                table: "QC_Receiving",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryApproveBy",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "ExpiryDateOfApprove",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "IsNearlyExpire",
                table: "QC_Receiving");

            migrationBuilder.AlterColumn<bool>(
                name: "ExpiryIsApprove",
                table: "QC_Receiving",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
