using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class makingpropNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_Campaign_CampaignId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campaign",
                table: "Campaign");

            migrationBuilder.RenameTable(
                name: "Campaign",
                newName: "campaigns");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "campaigns",
                newName: "DiscountPercentageValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_campaigns",
                table: "campaigns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_campaigns_CampaignId",
                table: "products",
                column: "CampaignId",
                principalTable: "campaigns",
                principalColumn: "Id");
            migrationBuilder.AlterColumn<decimal?>(
        name: "DiscountPercentageValue",
        table: "campaigns",
        nullable: true,
        oldClrType: typeof(decimal),
        oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_campaigns_CampaignId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_campaigns",
                table: "campaigns");

            migrationBuilder.RenameTable(
                name: "campaigns",
                newName: "Campaign");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentageValue",
                table: "Campaign",
                newName: "DiscountPercentage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campaign",
                table: "Campaign",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_Campaign_CampaignId",
                table: "products",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id");
        }
    }
}
