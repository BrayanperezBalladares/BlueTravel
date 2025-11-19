using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDuracionYCapacidadOfertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacidadMaxima",
                table: "Ofertas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CargoPorPersonaExtra",
                table: "Ofertas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DuracionDias",
                table: "Ofertas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonasIncluidas",
                table: "Ofertas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacidadMaxima",
                table: "Ofertas");

            migrationBuilder.DropColumn(
                name: "CargoPorPersonaExtra",
                table: "Ofertas");

            migrationBuilder.DropColumn(
                name: "DuracionDias",
                table: "Ofertas");

            migrationBuilder.DropColumn(
                name: "PersonasIncluidas",
                table: "Ofertas");
        }
    }
}
