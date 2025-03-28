using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewRelationWithCampaigm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_campaigns_CampaignId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_CampaignId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "products");

            migrationBuilder.CreateTable(
                name: "campaignProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaignProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaignProducts_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaignProducts_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaignProducts_CampaignId",
                table: "campaignProducts",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignProducts_ProductId",
                table: "campaignProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaignProducts");

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_CampaignId",
                table: "products",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_products_campaigns_CampaignId",
                table: "products",
                column: "CampaignId",
                principalTable: "campaigns",
                principalColumn: "Id");
        }
    }
}
