using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reserva_Restaurantes.Data.Migrations
{
    /// <inheritdoc />
    public partial class Reserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesas_Restaurantes_RestauranteId",
                table: "Mesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagamento_Reservas_ReservaId",
                table: "Pagamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Restaurantes_RestaurantesId",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "RestaurantesId",
                table: "Reservas",
                newName: "RestauranteId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_RestaurantesId",
                table: "Reservas",
                newName: "IX_Reservas_RestauranteId");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Restaurantes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Endereco",
                table: "Restaurantes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Reservas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantesFK",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "metodo",
                table: "Pagamento",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Pagamento",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "RestauranteId",
                table: "Mesas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesas_Restaurantes_RestauranteId",
                table: "Mesas",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagamento_Reservas_ReservaId",
                table: "Pagamento",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Restaurantes_RestauranteId",
                table: "Reservas",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesas_Restaurantes_RestauranteId",
                table: "Mesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagamento_Reservas_ReservaId",
                table: "Pagamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Restaurantes_RestauranteId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RestaurantesFK",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "RestauranteId",
                table: "Reservas",
                newName: "RestaurantesId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_RestauranteId",
                table: "Reservas",
                newName: "IX_Reservas_RestaurantesId");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Restaurantes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Endereco",
                table: "Restaurantes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "metodo",
                table: "Pagamento",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Pagamento",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RestauranteId",
                table: "Mesas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesas_Restaurantes_RestauranteId",
                table: "Mesas",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagamento_Reservas_ReservaId",
                table: "Pagamento",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Restaurantes_RestaurantesId",
                table: "Reservas",
                column: "RestaurantesId",
                principalTable: "Restaurantes",
                principalColumn: "Id");
        }
    }
}
