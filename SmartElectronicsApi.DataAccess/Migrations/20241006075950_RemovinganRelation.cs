using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovinganRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parametrGroups_ProductVariations_ProductVariationId",
                table: "parametrGroups");

            migrationBuilder.DropIndex(
                name: "IX_parametrGroups_ProductVariationId",
                table: "parametrGroups");

            migrationBuilder.DropColumn(
                name: "ProductVariationId",
                table: "parametrGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariationId",
                table: "parametrGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_parametrGroups_ProductVariationId",
                table: "parametrGroups",
                column: "ProductVariationId");

            migrationBuilder.AddForeignKey(
                name: "FK_parametrGroups_ProductVariations_ProductVariationId",
                table: "parametrGroups",
                column: "ProductVariationId",
                principalTable: "ProductVariations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
