using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class FeatPredefinedAdminRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "josipvaltner@gmail.com", "pass_jv", "Admin", "Josip_admin" },
                    { 2, "lukakovacevic@gmail.com", "pass_lk", "Admin", "Luka_admin" },
                    { 3, "borislaketic@gmail.com", "pass_bl", "Admin", "Boris_admin" },
                    { 4, "kopasztamas@gmail.com", "pass_kt", "Admin", "Tamas_admin" },
                    { 5, "urosmilinovic@gmail.com", "pass_um", "Admin", "Uros_admin" }
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
        }
    }
}
