using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingNullToFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "categories",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "categories");
        }
    }
}
