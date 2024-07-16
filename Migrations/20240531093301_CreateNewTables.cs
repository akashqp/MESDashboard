using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESDashboard.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LadleCompositionData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LadleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    C = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Si = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    S = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ti = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    V = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pb = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Al = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ni = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Co = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Nb = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    W = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    As = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ca = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ce = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sb = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    B = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    N = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Zn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FMg = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LadleCompositionData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PipeProductionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PipeNo = table.Column<int>(type: "int", nullable: false),
                    LadleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LadleCompositionDataId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipeProductionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipeProductionReports_LadleCompositionData_LadleCompositionDataId",
                        column: x => x.LadleCompositionDataId,
                        principalTable: "LadleCompositionData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipeProductionReports_LadleCompositionDataId",
                table: "PipeProductionReports",
                column: "LadleCompositionDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipeProductionReports");

            migrationBuilder.DropTable(
                name: "LadleCompositionData");
        }
    }
}
