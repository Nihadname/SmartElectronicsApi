using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class uuuujAjja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_brands_subcategories_SubCategoryId",
                table: "brands");

            migrationBuilder.DropIndex(
                name: "IX_brands_SubCategoryId",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "brands");

            migrationBuilder.CreateTable(
                name: "brandSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brandSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_brandSubCategories_brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_brandSubCategories_subcategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_brandSubCategories_BrandId",
                table: "brandSubCategories",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_brandSubCategories_SubCategoryId",
                table: "brandSubCategories",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brandSubCategories");

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "brands",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_brands_SubCategoryId",
                table: "brands",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_brands_subcategories_SubCategoryId",
                table: "brands",
                column: "SubCategoryId",
                principalTable: "subcategories",
                principalColumn: "Id");
        }
    }
}
