using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusFix.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReporteSolucion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSolucion",
                table: "Reportes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Solucion",
                table: "Reportes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaSolucion",
                table: "Reportes");

            migrationBuilder.DropColumn(
                name: "Solucion",
                table: "Reportes");
        }
    }
}
