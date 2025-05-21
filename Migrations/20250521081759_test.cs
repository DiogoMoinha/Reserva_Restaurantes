using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reserva_Restaurantes.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Restaurantes_RestaurantesId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_RestaurantesId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RestaurantesId",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "RestauranteFK",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_RestauranteFK",
                table: "Reservas",
                column: "RestauranteFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Restaurantes_RestauranteFK",
                table: "Reservas",
                column: "RestauranteFK",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Restaurantes_RestauranteFK",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_RestauranteFK",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RestauranteFK",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantesId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_RestaurantesId",
                table: "Reservas",
                column: "RestaurantesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Restaurantes_RestaurantesId",
                table: "Reservas",
                column: "RestaurantesId",
                principalTable: "Restaurantes",
                principalColumn: "Id");
        }
    }
}
