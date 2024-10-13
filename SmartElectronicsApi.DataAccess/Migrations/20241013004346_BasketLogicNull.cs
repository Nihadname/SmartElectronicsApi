using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BasketLogicNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_basketProducts_products_ProductId",
                table: "basketProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariationId",
                table: "basketProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "basketProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_basketProducts_products_ProductId",
                table: "basketProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_basketProducts_products_ProductId",
                table: "basketProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductVariationId",
                table: "basketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "basketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketProducts_products_ProductId",
                table: "basketProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
