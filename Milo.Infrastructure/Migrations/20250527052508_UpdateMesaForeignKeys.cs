using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Milo.Infrastructure.Migrations
{
    public partial class UpdateMesaForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mesas_pedidos_pedido_id",
                table: "mesas");

            migrationBuilder.AlterColumn<int>(
                name: "pedido_id",
                table: "mesas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_mesas_pedidos_pedido_id",
                table: "mesas",
                column: "pedido_id",
                principalTable: "pedidos",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mesas_pedidos_pedido_id",
                table: "mesas");

            migrationBuilder.AlterColumn<int>(
                name: "pedido_id",
                table: "mesas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mesas_pedidos_pedido_id",
                table: "mesas",
                column: "pedido_id",
                principalTable: "pedidos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
