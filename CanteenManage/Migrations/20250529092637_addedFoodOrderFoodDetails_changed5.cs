using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class addedFoodOrderFoodDetails_changed5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetail_Employees_EmployeeId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodOrders_FoodOrderId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetail_Foods_FoodId",
                table: "FoodOrderFoodDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodOrderFoodDetail",
                table: "FoodOrderFoodDetail");

            migrationBuilder.RenameTable(
                name: "FoodOrderFoodDetail",
                newName: "FoodOrderFoodDetails");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetail_FoodTypeId",
                table: "FoodOrderFoodDetails",
                newName: "IX_FoodOrderFoodDetails_FoodTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetail_FoodOrderId",
                table: "FoodOrderFoodDetails",
                newName: "IX_FoodOrderFoodDetails_FoodOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetail_FoodId",
                table: "FoodOrderFoodDetails",
                newName: "IX_FoodOrderFoodDetails_FoodId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetail_EmployeeId",
                table: "FoodOrderFoodDetails",
                newName: "IX_FoodOrderFoodDetails_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodOrderFoodDetails",
                table: "FoodOrderFoodDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetails_Employees_EmployeeId",
                table: "FoodOrderFoodDetails",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetails_FoodOrders_FoodOrderId",
                table: "FoodOrderFoodDetails",
                column: "FoodOrderId",
                principalTable: "FoodOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetails_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetails",
                column: "FoodTypeId",
                principalTable: "FoodTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetails_Foods_FoodId",
                table: "FoodOrderFoodDetails",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetails_Employees_EmployeeId",
                table: "FoodOrderFoodDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetails_FoodOrders_FoodOrderId",
                table: "FoodOrderFoodDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetails_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrderFoodDetails_Foods_FoodId",
                table: "FoodOrderFoodDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodOrderFoodDetails",
                table: "FoodOrderFoodDetails");

            migrationBuilder.RenameTable(
                name: "FoodOrderFoodDetails",
                newName: "FoodOrderFoodDetail");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetails_FoodTypeId",
                table: "FoodOrderFoodDetail",
                newName: "IX_FoodOrderFoodDetail_FoodTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetails_FoodOrderId",
                table: "FoodOrderFoodDetail",
                newName: "IX_FoodOrderFoodDetail_FoodOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetails_FoodId",
                table: "FoodOrderFoodDetail",
                newName: "IX_FoodOrderFoodDetail_FoodId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrderFoodDetails_EmployeeId",
                table: "FoodOrderFoodDetail",
                newName: "IX_FoodOrderFoodDetail_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodOrderFoodDetail",
                table: "FoodOrderFoodDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetail_Employees_EmployeeId",
                table: "FoodOrderFoodDetail",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodOrders_FoodOrderId",
                table: "FoodOrderFoodDetail",
                column: "FoodOrderId",
                principalTable: "FoodOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetail_FoodTypes_FoodTypeId",
                table: "FoodOrderFoodDetail",
                column: "FoodTypeId",
                principalTable: "FoodTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrderFoodDetail_Foods_FoodId",
                table: "FoodOrderFoodDetail",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }
    }
}
