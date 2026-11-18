using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Danucite360.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebtSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Period = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    BudgetYear = table.Column<int>(type: "int", nullable: false),
                    AsOf = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TotalDebt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DebtToGdpPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DomesticDebt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DomesticSharePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExternalDebt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExternalSharePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GuaranteedDebt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GuaranteedToGdpPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StateDebt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StateDebtSharePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EurDenominatedPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FixedRatePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgInterestPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgMaturity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtAuctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DebtSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Maturity = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    YieldPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtAuctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtAuctions_DebtSnapshots_DebtSnapshotId",
                        column: x => x.DebtSnapshotId,
                        principalTable: "DebtSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtBreakdownItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DebtSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtBreakdownItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtBreakdownItems_DebtSnapshots_DebtSnapshotId",
                        column: x => x.DebtSnapshotId,
                        principalTable: "DebtSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtTrendPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DebtSnapshotId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Domestic = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    External = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtTrendPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtTrendPoints_DebtSnapshots_DebtSnapshotId",
                        column: x => x.DebtSnapshotId,
                        principalTable: "DebtSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtAuctions_DebtSnapshotId",
                table: "DebtAuctions",
                column: "DebtSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtBreakdownItems_DebtSnapshotId",
                table: "DebtBreakdownItems",
                column: "DebtSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtSnapshots_BudgetYear_Period",
                table: "DebtSnapshots",
                columns: new[] { "BudgetYear", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtTrendPoints_DebtSnapshotId",
                table: "DebtTrendPoints",
                column: "DebtSnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtAuctions");

            migrationBuilder.DropTable(
                name: "DebtBreakdownItems");

            migrationBuilder.DropTable(
                name: "DebtTrendPoints");

            migrationBuilder.DropTable(
                name: "DebtSnapshots");
        }
    }
}
