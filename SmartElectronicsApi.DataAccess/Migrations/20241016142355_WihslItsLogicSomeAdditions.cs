using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class WihslItsLogicSomeAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wishListProducts_products_ProductId",
                table: "wishListProducts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "wishListProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "wishListProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "wishListProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_wishListProducts_ProductVariationId",
                table: "wishListProducts",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_wishListProducts_ProductVariations_ProductVariationId",
                table: "wishListProducts",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_wishListProducts_products_ProductId",
                table: "wishListProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wishListProducts_ProductVariations_ProductVariationId",
                table: "wishListProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_wishListProducts_products_ProductId",
                table: "wishListProducts");

            migrationBuilder.DropIndex(
                name: "IX_wishListProducts_ProductVariationId",
                table: "wishListProducts");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "wishListProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "wishListProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "wishListProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_wishListProducts_products_ProductId",
                table: "wishListProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
