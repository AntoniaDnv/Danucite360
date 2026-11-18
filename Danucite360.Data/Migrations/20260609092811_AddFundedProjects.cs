using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Danucite360.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFundedProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundedProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CompletionPercent = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BudgetYear = table.Column<int>(type: "int", nullable: false),
                    IsDemo = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    BudgetSourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundedProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundedProjects_BudgetSources_BudgetSourceId",
                        column: x => x.BudgetSourceId,
                        principalTable: "BudgetSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FundedProjects_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFundings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FundedProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFundings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFundings_FundedProjects_FundedProjectId",
                        column: x => x.FundedProjectId,
                        principalTable: "FundedProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundedProjects_BudgetSourceId",
                table: "FundedProjects",
                column: "BudgetSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FundedProjects_RegionId",
                table: "FundedProjects",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_FundedProjects_Slug",
                table: "FundedProjects",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFundings_FundedProjectId",
                table: "ProjectFundings",
                column: "FundedProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectFundings");

            migrationBuilder.DropTable(
                name: "FundedProjects");
        }
    }
}
