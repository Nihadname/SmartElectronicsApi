using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BasketLogicAddingVariation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "basketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_basketProducts_ProductVariationId",
                table: "basketProducts",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketProducts_ProductVariations_ProductVariationId",
                table: "basketProducts");

            migrationBuilder.DropIndex(
                name: "IX_basketProducts_ProductVariationId",
                table: "basketProducts");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "basketProducts");
        }
    }
}
