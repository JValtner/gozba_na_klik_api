using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRestaurantModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Restaurants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Restaurants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Restaurants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ClosedDates",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Reason",
                value: null);

            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Reason",
                value: null);

            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Reason",
                value: null);

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Description", "Phone" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Description", "Phone" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Description", "Phone" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ClosedDates");
        }
    }
}
