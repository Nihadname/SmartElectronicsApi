using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LoyarTierListAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "loyaltyTier",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loyaltyTier",
                table: "AspNetUsers");
        }
    }
}
