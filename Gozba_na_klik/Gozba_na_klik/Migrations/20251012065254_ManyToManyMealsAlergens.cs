using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyMealsAlergens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alergens_Meals_MealId",
                table: "Alergens");

            migrationBuilder.DropIndex(
                name: "IX_Alergens_MealId",
                table: "Alergens");

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

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "Alergens");

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
                    table.ForeignKey(
                        name: "FK_MealAlergens_Meals_MealsId",
                        column: x => x.MealsId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Fish");

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Soy");

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Crustaceans");

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Sesame");

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "Mustard");

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
                name: "IX_MealAlergens_MealsId",
                table: "MealAlergens",
                column: "MealsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealAlergens");

            migrationBuilder.AddColumn<int>(
                name: "MealId",
                table: "Alergens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 1,
                column: "MealId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 2,
                column: "MealId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 3,
                column: "MealId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "MealId", "Name" },
                values: new object[] { 2, "Gluten" });

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "MealId", "Name" },
                values: new object[] { 2, "Milk" });

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "MealId", "Name" },
                values: new object[] { 4, "Fish" });

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "MealId", "Name" },
                values: new object[] { 5, "Soy" });

            migrationBuilder.UpdateData(
                table: "Alergens",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "MealId", "Name" },
                values: new object[] { 5, "Crustaceans" });

            migrationBuilder.InsertData(
                table: "Alergens",
                columns: new[] { "Id", "MealId", "Name" },
                values: new object[,]
                {
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

            migrationBuilder.CreateIndex(
                name: "IX_Alergens_MealId",
                table: "Alergens",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alergens_Meals_MealId",
                table: "Alergens",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
