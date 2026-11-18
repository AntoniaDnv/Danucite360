using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Danucite360.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectLocationAndSpending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "FundedProjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "FundedProjects",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "FundedProjects",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Spent",
                table: "FundedProjects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "FundedProjects");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "FundedProjects");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "FundedProjects");

            migrationBuilder.DropColumn(
                name: "Spent",
                table: "FundedProjects");
        }
    }
}
