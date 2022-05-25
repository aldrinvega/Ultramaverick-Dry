using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ELIXIR.DATA.Migrations
{
    public partial class AddNewTableinPreparation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transformation_Preparation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransformId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    QuantityNeeded = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Batch = table.Column<int>(type: "int", nullable: false),
                    WeighingScale = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transformation_Preparation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transformation_Preparation");
        }
    }
}
