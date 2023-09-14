using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddRejectEntityandAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AllowableDate",
                table: "ReturnedItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RejectedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestRequestId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowableDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Disposition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RejectedItems_LabTestRequests_LabTestRequestId",
                        column: x => x.LabTestRequestId,
                        principalTable: "LabTestRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RejectedItems_LabTestRequestId",
                table: "RejectedItems",
                column: "LabTestRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RejectedItems");

            migrationBuilder.DropColumn(
                name: "AllowableDate",
                table: "ReturnedItems");
        }
    }
}
