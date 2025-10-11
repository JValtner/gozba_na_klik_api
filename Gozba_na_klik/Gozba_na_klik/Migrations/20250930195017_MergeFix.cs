using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class MergeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Reason",
                value: "Christmas");

            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Reason",
                value: "New Year");

            migrationBuilder.UpdateData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Reason",
                value: "Independence Day");

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Phone" },
                values: new object[] { "Some address 1", "123456" });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Phone" },
                values: new object[] { "Some address 2", "234567" });

            migrationBuilder.UpdateData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Phone" },
                values: new object[] { "Some address 3", "345678" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
