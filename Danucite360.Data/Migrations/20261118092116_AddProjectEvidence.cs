using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Danucite360.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectEvidence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundedProjectId = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DocumentTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDemo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEvidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectEvidence_FundedProjects_FundedProjectId",
                        column: x => x.FundedProjectId,
                        principalTable: "FundedProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEvidence_FundedProjectId",
                table: "ProjectEvidence",
                column: "FundedProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectEvidence");
        }
    }
}
