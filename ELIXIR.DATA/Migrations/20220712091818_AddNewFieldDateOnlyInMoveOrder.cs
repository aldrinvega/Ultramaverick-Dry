using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewFieldDateOnlyInMoveOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApproveDateTempo",
                table: "MoveOrders",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDateTempo",
                table: "MoveOrders",
                type: "Date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveDateTempo",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "RejectedDateTempo",
                table: "MoveOrders");
        }
    }
}
