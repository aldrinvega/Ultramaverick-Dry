using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class DatabaseAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        { 

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListStrings_QC_Receiving_PO_ReceivingId",
                table: "CheckListStrings",
                column: "PO_ReceivingId",
                principalTable: "QC_Receiving",
                principalColumn: "PO_Summary_Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
