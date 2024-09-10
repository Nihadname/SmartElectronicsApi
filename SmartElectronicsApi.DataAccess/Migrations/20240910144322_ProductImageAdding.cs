using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProductImageAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "productImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_productImages_ProductVariationId",
                table: "productImages",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_productImages_ProductVariations_ProductVariationId",
                table: "productImages",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productImages_ProductVariations_ProductVariationId",
                table: "productImages");

            migrationBuilder.DropIndex(
                name: "IX_productImages_ProductVariationId",
                table: "productImages");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "productImages");
        }
    }
}
