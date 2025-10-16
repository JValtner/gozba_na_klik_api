using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Entrance = table.Column<string>(type: "text", nullable: true),
                    Floor = table.Column<string>(type: "text", nullable: true),
                    Apartment = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alergens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClosedDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    MealId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAddons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealAlergens",
                columns: table => new
                {
                    AlergensId = table.Column<int>(type: "integer", nullable: false),
                    MealsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAlergens", x => new { x.AlergensId, x.MealsId });
                    table.ForeignKey(
                        name: "FK_MealAlergens_Alergens_AlergensId",
                        column: x => x.AlergensId,
                        principalTable: "Alergens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UserImage = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSchedules_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alergens",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Gluten" },
                    { 2, "Eggs" },
                    { 3, "Milk" },
                    { 4, "Fish" },
                    { 5, "Soy" },
                    { 6, "Crustaceans" },
                    { 7, "Sesame" },
                    { 8, "Mustard" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "Password", "RestaurantId", "Role", "UserImage", "Username" },
                values: new object[,]
                {
                    { 1, "josipvaltner@gmail.com", true, "pass_jv", null, "Admin", null, "Josip_admin" },
                    { 2, "lukakovacevic@gmail.com", true, "pass_lk", null, "Admin", null, "Luka_admin" },
                    { 3, "borislaketic@gmail.com", true, "pass_bl", null, "Admin", null, "Boris_admin" },
                    { 4, "kopasztamas@gmail.com", true, "pass_kt", null, "Admin", null, "Tamas_admin" },
                    { 5, "urosmilinovic@gmail.com", true, "pass_um", null, "Admin", null, "Uros_admin" },
                    { 7, "milan.owner@example.com", true, "pass_mo", null, "RestaurantOwner", null, "Milan_owner" },
                    { 8, "ana.owner@example.com", true, "pass_ao", null, "RestaurantOwner", null, "Ana_owner" },
                    { 9, "ivan.owner@example.com", true, "pass_io", null, "RestaurantOwner", null, "Ivan_owner" }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Address", "CreatedAt", "Description", "Name", "OwnerId", "Phone", "PhotoUrl", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Some address 1", new DateTime(2025, 9, 28, 8, 0, 0, 0, DateTimeKind.Utc), "Authentic Italian dishes made with fresh ingredients.", "Bella Italia", 7, "123456", "...", null },
                    { 2, "Some address 2", new DateTime(2025, 9, 28, 8, 30, 0, 0, DateTimeKind.Utc), "Authentic Japanese dishes made with fresh ingredients.", "Sushi Master", 8, "234567", "...", null },
                    { 3, "Some address 3", new DateTime(2025, 9, 28, 9, 0, 0, 0, DateTimeKind.Utc), "Authentic Ausie dishes made with fresh ingredients.", "Grill House", 9, "345678", "...", null }
                });

            migrationBuilder.InsertData(
                table: "ClosedDates",
                columns: new[] { "Id", "Date", "Reason", "RestaurantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Christmas", 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Year", 2 },
                    { 3, new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Independence Day", 3 }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "Price", "RestaurantId" },
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "Password", "RestaurantId", "Role", "UserImage", "Username" },
                values: new object[,]
                {
                    { 10, "petar.employee@example.com", true, "pass_pe", 1, "RestaurantEmployee", null, "Petar_employee" },
                    { 11, "marko.delivery@example.com", true, "pass_md", 1, "DeliveryPerson", null, "Marko_delivery" },
                    { 12, "ana.employee@example.com", true, "pass_ae", 2, "RestaurantEmployee", null, "Ana_employee" },
                    { 13, "jovan.delivery@example.com", true, "pass_jd", 2, "DeliveryPerson", null, "Jovan_delivery" },
                    { 14, "nikola.employee@example.com", true, "pass_ne", 3, "RestaurantEmployee", null, "Nikola_employee" },
                    { 15, "sara.employee@example.com", false, "pass_se", 1, "RestaurantEmployee", null, "Sara_employee" }
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

            migrationBuilder.InsertData(
                table: "MealAddons",
                columns: new[] { "Id", "IsActive", "MealId", "Name", "Price", "Type" },
                values: new object[,]
                {
                    { 1, true, 2, "Extra Cheese", 120m, "chosen" },
                    { 2, false, 1, "Garlic Bread", 150m, "independent" },
                    { 3, true, 3, "Parmesan", 80m, "chosen" },
                    { 4, true, 1, "Extra Sauce", 100m, "chosen" },
                    { 5, false, 2, "Chili Flakes", 50m, "independent" },
                    { 6, false, 4, "Soy Sauce", 60m, "independent" },
                    { 7, false, 5, "Extra Wasabi", 70m, "chosen" },
                    { 8, false, 6, "Ginger", 50m, "independent" },
                    { 9, false, 7, "Boiled Egg", 120m, "chosen" },
                    { 10, true, 7, "Extra Pork", 200m, "chosen" },
                    { 11, false, 10, "Fries", 180m, "independent" },
                    { 12, false, 10, "Onion Rings", 160m, "independent" },
                    { 13, true, 8, "BBQ Sauce", 90m, "chosen" },
                    { 14, false, 8, "Coleslaw", 130m, "independent" },
                    { 15, false, 9, "Grilled Vegetables", 150m, "independent" },
                    { 16, true, 5, "Spicy Mayo", 80m, "chosen" },
                    { 17, true, 4, "Teriyaki Sauce", 90m, "chosen" },
                    { 18, false, 7, "Extra Noodles", 140m, "chosen" },
                    { 19, true, 10, "Extra Beef", 220m, "chosen" },
                    { 20, false, 9, "Caesar Salad", 190m, "independent" }
                });

            migrationBuilder.InsertData(
                table: "MealAlergens",
                columns: new[] { "AlergensId", "MealsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 10 },
                    { 2, 1 },
                    { 2, 3 },
                    { 3, 1 },
                    { 3, 2 },
                    { 3, 3 },
                    { 3, 9 },
                    { 4, 4 },
                    { 5, 5 },
                    { 5, 7 },
                    { 5, 8 },
                    { 5, 9 },
                    { 6, 5 },
                    { 7, 5 },
                    { 8, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedDates_RestaurantId",
                table: "ClosedDates",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAddons_MealId",
                table: "MealAddons",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealAlergens_MealsId",
                table: "MealAlergens",
                column: "MealsId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RestaurantId",
                table: "Meals",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_OwnerId",
                table: "Restaurants",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RestaurantId",
                table: "Users",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_RestaurantId",
                table: "WorkSchedules",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedDates_Restaurants_RestaurantId",
                table: "ClosedDates",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealAddons_Meals_MealId",
                table: "MealAddons",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealAlergens_Meals_MealsId",
                table: "MealAlergens",
                column: "MealsId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meals_Restaurants_RestaurantId",
                table: "Meals",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Restaurants_RestaurantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "ClosedDates");

            migrationBuilder.DropTable(
                name: "MealAddons");

            migrationBuilder.DropTable(
                name: "MealAlergens");

            migrationBuilder.DropTable(
                name: "WorkSchedules");

            migrationBuilder.DropTable(
                name: "Alergens");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
