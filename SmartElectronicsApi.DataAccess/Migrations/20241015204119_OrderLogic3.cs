using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OrderLogic3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariationId",
                table: "OrderItems",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariations_ProductVariationId",
                table: "OrderItems",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariations_ProductVariationId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductVariationId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "OrderItems");
        }
    }
}
