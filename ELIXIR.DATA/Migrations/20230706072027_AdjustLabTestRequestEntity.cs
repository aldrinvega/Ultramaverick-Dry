using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AdjustLabTestRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateNeeded",
                table: "LabTestRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRequested",
                table: "LabTestRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "LabTestRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "LabTestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "LabTestRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateNeeded",
                table: "LabTestRequests");

            migrationBuilder.DropColumn(
                name: "DateRequested",
                table: "LabTestRequests");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "LabTestRequests");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "LabTestRequests");

            migrationBuilder.DropColumn(
                name: "TestType",
                table: "LabTestRequests");
        }
    }
}
