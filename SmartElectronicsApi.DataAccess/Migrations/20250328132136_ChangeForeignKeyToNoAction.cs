using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeForeignKeyToNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_campaignProducts_products_ProductId",
                table: "campaignProducts");

            // Create the new foreign key constraint with NoAction
            migrationBuilder.AddForeignKey(
                name: "FK_campaignProducts_products_ProductId",
                table: "campaignProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);  // Set NoAction to prevent cascading delete
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to the previous behavior (Cascade)
            migrationBuilder.DropForeignKey(
                name: "FK_campaignProducts_products_ProductId",
                table: "campaignProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_campaignProducts_products_ProductId",
                table: "campaignProducts",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);  // Restore Cascade
        }
    }
}
