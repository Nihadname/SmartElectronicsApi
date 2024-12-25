using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewChangeGuestOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_guestOrders_Name",
                table: "guestOrders");

            migrationBuilder.DropIndex(
                name: "IX_guestOrders_SurName",
                table: "guestOrders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "guestOrders");

            migrationBuilder.RenameColumn(
                name: "SurName",
                table: "guestOrders",
                newName: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_FullName",
                table: "guestOrders",
                column: "FullName",
                unique: true,
                filter: "[FullName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_guestOrders_FullName",
                table: "guestOrders");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "guestOrders",
                newName: "SurName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "guestOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_Name",
                table: "guestOrders",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_SurName",
                table: "guestOrders",
                column: "SurName",
                unique: true,
                filter: "[SurName] IS NOT NULL");
        }
    }
}
