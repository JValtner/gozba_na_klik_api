using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddonAlergenSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Alergens",
                columns: new[] { "Id", "MealId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Gluten" },
                    { 2, 1, "Eggs" },
                    { 3, 1, "Milk" },
                    { 4, 2, "Gluten" },
                    { 5, 2, "Milk" },
                    { 6, 4, "Fish" },
                    { 7, 5, "Soy" },
                    { 8, 5, "Crustaceans" },
                    { 9, 6, "Fish" },
                    { 10, 7, "Soy" },
                    { 11, 5, "Sesame" },
                    { 12, 8, "Mustard" },
                    { 13, 8, "Soy" },
                    { 14, 9, "Milk" },
                    { 15, 10, "Gluten" },
                    { 16, 7, "Soy" },
                    { 17, 4, "Fish" },
                    { 18, 3, "Eggs" },
                    { 19, 3, "Milk" },
                    { 20, 9, "Soy" }
                });

            migrationBuilder.InsertData(
                table: "MealAddons",
                columns: new[] { "Id", "MealId", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 1, 2, "Extra Cheese", 120m, "chosen" },
                    { 2, 1, "Garlic Bread", 150m, "independent" },
                    { 3, 3, "Parmesan", 80m, "chosen" },
                    { 4, 1, "Extra Sauce", 100m, "chosen" },
                    { 5, 2, "Chili Flakes", 50m, "independent" },
                    { 6, 4, "Soy Sauce", 60m, "independent" },
                    { 7, 5, "Extra Wasabi", 70m, "chosen" },
                    { 8, 6, "Ginger", 50m, "independent" },
                    { 9, 7, "Boiled Egg", 120m, "chosen" },
                    { 10, 7, "Extra Pork", 200m, "chosen" },
                    { 11, 10, "Fries", 180m, "independent" },
                    { 12, 10, "Onion Rings", 160m, "independent" },
                    { 13, 8, "BBQ Sauce", 90m, "chosen" },
                    { 14, 8, "Coleslaw", 130m, "independent" },
                    { 15, 9, "Grilled Vegetables", 150m, "independent" },
                    { 16, 5, "Spicy Mayo", 80m, "chosen" },
                    { 17, 4, "Teriyaki Sauce", 90m, "chosen" },
                    { 18, 7, "Extra Noodles", 140m, "chosen" },
                    { 19, 10, "Extra Beef", 220m, "chosen" },
                    { 20, 9, "Caesar Salad", 190m, "independent" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MealAddons",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
