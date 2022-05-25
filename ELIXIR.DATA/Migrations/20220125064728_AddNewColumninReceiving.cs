using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewColumninReceiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelBy",
                table: "QC_Receiving",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "QC_Receiving",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "QC_Receiving",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "QC_Receiving",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalReject",
                table: "QC_Receiving",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelBy",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "QC_Receiving");

            migrationBuilder.DropColumn(
                name: "TotalReject",
                table: "QC_Receiving");
        }
    }
}
