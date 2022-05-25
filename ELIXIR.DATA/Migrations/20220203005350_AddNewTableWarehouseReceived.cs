using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableWarehouseReceived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseReceived",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PO_Number = table.Column<int>(type: "int", nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ManufacturingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Expiration = table.Column<DateTime>(type: "Date", nullable: false),
                    ExpirationDays = table.Column<int>(type: "int", nullable: false),
                    ActualDelivered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityGood = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualReject = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LotCategoryId = table.Column<int>(type: "int", nullable: false),
                    UpdatedStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualGood = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsWarehouseReceive = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseReceived", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseReceived_LotCategories_LotCategoryId",
                        column: x => x.LotCategoryId,
                        principalTable: "LotCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReceived_LotCategoryId",
                table: "WarehouseReceived",
                column: "LotCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseReceived");
        }
    }
}
