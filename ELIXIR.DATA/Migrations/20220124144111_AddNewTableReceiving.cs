using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableReceiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QC_Receiving",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturing_Date = table.Column<DateTime>(type: "Date", nullable: false),
                    Expected_Delivery = table.Column<int>(type: "int", nullable: false),
                    Expiry_Date = table.Column<DateTime>(type: "Date", nullable: false),
                    Actual_Delivered = table.Column<int>(type: "int", nullable: false),
                    Batch_No = table.Column<int>(type: "int", nullable: false),
                    Truck_Approval1 = table.Column<bool>(type: "bit", nullable: false),
                    Truck_Approval2 = table.Column<bool>(type: "bit", nullable: false),
                    Truck_Approval3 = table.Column<bool>(type: "bit", nullable: false),
                    Truck_Approval4 = table.Column<bool>(type: "bit", nullable: false),
                    Truck_Approval1_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Truck_Approval2_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Truck_Approval3_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Truck_Approval4_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unloading_Approval1 = table.Column<bool>(type: "bit", nullable: false),
                    Unloading_Approval2 = table.Column<bool>(type: "bit", nullable: false),
                    Unloading_Approval3 = table.Column<bool>(type: "bit", nullable: false),
                    Unloading_Approval4 = table.Column<bool>(type: "bit", nullable: false),
                    Unloading_Approval1_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unloading_Approval2_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unloading_Approval3_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unloading_Approval4_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checking_Approval1 = table.Column<bool>(type: "bit", nullable: false),
                    Checking_Approval2 = table.Column<bool>(type: "bit", nullable: false),
                    Checking_Approval1_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checking_Approval2_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QA_Approval = table.Column<bool>(type: "bit", nullable: false),
                    QA_Approval_Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QC_Receiving", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QC_Receiving");
        }
    }
}
