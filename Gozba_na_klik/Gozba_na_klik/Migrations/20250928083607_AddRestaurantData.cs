using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "CreatedAt", "Name", "OwnerId", "PhotoUrl", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 28, 8, 0, 0, 0, DateTimeKind.Utc), "Bella Italia", 7, "...", null },
                    { 2, new DateTime(2025, 9, 28, 8, 30, 0, 0, DateTimeKind.Utc), "Sushi Master", 8, "...", null },
                    { 3, new DateTime(2025, 9, 28, 9, 0, 0, 0, DateTimeKind.Utc), "Grill House", 9, "...", null }
                });

            migrationBuilder.InsertData(
                table: "ClosedDates",
                columns: new[] { "Id", "Date", "RestaurantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 3, new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.InsertData(
                table: "WorkSchedules",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "OpenTime", "RestaurantId" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 21, 0, 0, 0), 1, new TimeSpan(0, 9, 0, 0, 0), 1 },
                    { 2, new TimeSpan(0, 21, 0, 0, 0), 2, new TimeSpan(0, 9, 0, 0, 0), 1 },
                    { 3, new TimeSpan(0, 22, 0, 0, 0), 1, new TimeSpan(0, 11, 0, 0, 0), 2 },
                    { 4, new TimeSpan(0, 22, 0, 0, 0), 2, new TimeSpan(0, 11, 0, 0, 0), 2 },
                    { 5, new TimeSpan(0, 20, 0, 0, 0), 1, new TimeSpan(0, 10, 0, 0, 0), 3 },
                    { 6, new TimeSpan(0, 20, 0, 0, 0), 2, new TimeSpan(0, 10, 0, 0, 0), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ClosedDates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
