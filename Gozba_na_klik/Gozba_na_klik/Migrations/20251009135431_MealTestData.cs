using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class MealTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "Price", "RestaurantId" },
                values: new object[,]
                {
                    { 1, "Classic Italian pasta with pancetta, egg, and pecorino cheese.", "...", "Spaghetti Carbonara", 950m, 1 },
                    { 2, "Fresh mozzarella, tomato sauce, and basil on a wood-fired crust.", "...", "Margherita Pizza", 890m, 1 },
                    { 3, "Layered pasta with beef ragù, bechamel sauce, and parmesan.", "...", "Lasagna al Forno", 1100m, 1 },
                    { 4, "Fresh salmon on seasoned rice, served with wasabi.", "...", "Salmon Nigiri", 620m, 2 },
                    { 5, "Crab, avocado, and cucumber rolled in sesame rice.", "...", "California Roll", 750m, 2 },
                    { 6, "Thinly sliced tuna served with soy sauce and wasabi.", "...", "Tuna Sashimi", 980m, 2 },
                    { 7, "Rich miso broth with noodles, egg, and pork slices.", "...", "Ramen Bowl", 1100m, 2 },
                    { 8, "Slow-cooked ribs with tangy BBQ sauce.", "...", "BBQ Ribs", 1450m, 3 },
                    { 9, "Juicy grilled chicken with seasonal vegetables.", "...", "Grilled Chicken Breast", 980m, 3 },
                    { 10, "Classic beef burger with cheddar, lettuce, and tomato.", "...", "Beef Burger", 890m, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
