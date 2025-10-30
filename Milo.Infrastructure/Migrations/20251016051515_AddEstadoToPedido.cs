using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Milo.Infrastructure.Migrations
{
    public partial class AddEstadoToPedido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estado",
                table: "pedidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "observaciones",
                table: "pedidos",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "observaciones",
                table: "pedidos");
        }
    }
}
