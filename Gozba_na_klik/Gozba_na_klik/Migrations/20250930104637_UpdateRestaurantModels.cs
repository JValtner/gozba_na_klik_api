using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    public partial class UpdateRestaurantModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use SQL to conditionally add columns only if they do not exist

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

            // Reason mezőt nyugodtan NULL-ra állíthatjuk, mert nullable
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

            // FONTOS: NEM állítunk Description-t NULL-ra
            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Phone" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Phone" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Phone" },
                values: new object[] { null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
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
