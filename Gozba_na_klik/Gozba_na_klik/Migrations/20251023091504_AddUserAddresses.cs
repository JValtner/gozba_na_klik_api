using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultAddressId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "DefaultAddressId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultAddressId",
                table: "Users",
                column: "DefaultAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_DefaultAddressId",
                table: "Users",
                column: "DefaultAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_DefaultAddressId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DefaultAddressId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "DefaultAddressId",
                table: "Users");
        }
    }
}
