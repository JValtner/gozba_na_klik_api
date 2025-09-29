using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "RestaurantId", "Role", "UserImage", "Username" },
                values: new object[,]
                {
                    { 1, "josipvaltner@gmail.com", "pass_jv", null, "Admin", null, "Josip_admin" },
                    { 2, "lukakovacevic@gmail.com", "pass_lk", null, "Admin", null, "Luka_admin" },
                    { 3, "borislaketic@gmail.com", "pass_bl", null, "Admin", null, "Boris_admin" },
                    { 4, "kopasztamas@gmail.com", "pass_kt", null, "Admin", null, "Tamas_admin" },
                    { 5, "urosmilinovic@gmail.com", "pass_um", null, "Admin", null, "Uros_admin" },
                    { 7, "milan.owner@example.com", "pass_mo", null, "RestaurantOwner", null, "Milan_owner" },
                    { 8, "ana.owner@example.com", "pass_ao", null, "RestaurantOwner", null, "Ana_owner" },
                    { 9, "ivan.owner@example.com", "pass_io", null, "RestaurantOwner", null, "Ivan_owner" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
