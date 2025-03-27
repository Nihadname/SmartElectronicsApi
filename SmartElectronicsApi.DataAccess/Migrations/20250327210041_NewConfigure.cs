using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartElectronicsApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewConfigure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_guestOrders_EmailAdress",
                table: "guestOrders");

            migrationBuilder.DropIndex(
                name: "IX_guestOrders_FullName",
                table: "guestOrders");

            migrationBuilder.DropIndex(
                name: "IX_guestOrders_PhoneNumber",
                table: "guestOrders");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "guestOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "guestOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailAdress",
                table: "guestOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "guestOrders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "guestOrders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailAdress",
                table: "guestOrders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_EmailAdress",
                table: "guestOrders",
                column: "EmailAdress",
                unique: true,
                filter: "[EmailAdress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_FullName",
                table: "guestOrders",
                column: "FullName",
                unique: true,
                filter: "[FullName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_guestOrders_PhoneNumber",
                table: "guestOrders",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }
    }
}
