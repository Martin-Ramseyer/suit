using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace suitMvc.Migrations
{
    /// <inheritdoc />
    public partial class addColumnsPasoAndPulsera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pulsera",
                table: "Invitados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "paso",
                table: "Invitados",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pulsera",
                table: "Invitados");

            migrationBuilder.DropColumn(
                name: "paso",
                table: "Invitados");
        }
    }
}
           