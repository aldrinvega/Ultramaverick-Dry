using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddAcceptedLabRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disposition",
                table: "LabTestRequests");

            migrationBuilder.CreateTable(
                name: "AcceptedLabRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestRequestsId = table.Column<int>(type: "int", nullable: false),
                    ExtendedExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedLabRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcceptedLabRequests_LabTestRequests_LabTestRequestsId",
                        column: x => x.LabTestRequestsId,
                        principalTable: "LabTestRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedLabRequests_LabTestRequestsId",
                table: "AcceptedLabRequests",
                column: "LabTestRequestsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedLabRequests");

            migrationBuilder.AddColumn<string>(
                name: "Disposition",
                table: "LabTestRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
