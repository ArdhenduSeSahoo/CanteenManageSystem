using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class addedFoodOrderFoodDetails_changedOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FoodTypeId",
                table: "FoodOrderFoodDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrderFoodDetail_FoodTypeId",
                table: "FoodOrderFoodDetail",
                column: "FoodTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetail",
                column: "FoodTypeId",
                principalTable: "FoodTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropIndex(
                name: "IX_FoodOrderFoodDetail_FoodTypeId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropColumn(
                name: "FoodTypeId",
                table: "FoodOrderFoodDetail");
        }
    }
}
