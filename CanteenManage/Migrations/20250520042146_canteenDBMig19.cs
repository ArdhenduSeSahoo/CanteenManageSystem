using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Foods_FoodId",
                table: "FoodOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReviewDetails_Foods_FoodId",
                table: "FoodReviewDetails");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "FoodReviewDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "FoodOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "FoodOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Foods_FoodId",
                table: "FoodOrders",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReviewDetails_Foods_FoodId",
                table: "FoodReviewDetails",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Foods_FoodId",
                table: "FoodOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReviewDetails_Foods_FoodId",
                table: "FoodReviewDetails");

            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "FoodOrders");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "FoodReviewDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "FoodOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Foods_FoodId",
                table: "FoodOrders",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReviewDetails_Foods_FoodId",
                table: "FoodReviewDetails",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
