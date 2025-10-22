using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gozba_na_klik.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPersonToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveOrderId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryPersonId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                    CourierId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantRating = table.Column<int>(type: "integer", nullable: false),
                    RestaurantComment = table.Column<string>(type: "text", nullable: true),
                    RestaurantPhotoUrl = table.Column<string>(type: "text", nullable: true),
                    CourierRating = table.Column<int>(type: "integer", nullable: false),
                    CourierComment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "ActiveOrderId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActiveOrderId",
                table: "Users",
                column: "ActiveOrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Orders_ActiveOrderId",
                table: "Users",
                column: "ActiveOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Orders_ActiveOrderId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Users_ActiveOrderId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActiveOrderId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeliveryPersonId",
                table: "Orders");
        }
    }
}
