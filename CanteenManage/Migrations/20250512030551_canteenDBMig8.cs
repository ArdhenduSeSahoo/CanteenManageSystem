using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Employes_EmployeId",
                table: "FoodOrders");

            migrationBuilder.RenameColumn(
                name: "EmployPrice",
                table: "Foods",
                newName: "EmployeePrice");

            migrationBuilder.RenameColumn(
                name: "TotalEmployPrice",
                table: "FoodOrders",
                newName: "TotalEmployeePrice");

            migrationBuilder.RenameColumn(
                name: "EmployeId",
                table: "FoodOrders",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrders_EmployeId",
                table: "FoodOrders",
                newName: "IX_FoodOrders_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Employes_EmployeeId",
                table: "FoodOrders",
                column: "EmployeeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Employes_EmployeeId",
                table: "FoodOrders");

            migrationBuilder.RenameColumn(
                name: "EmployeePrice",
                table: "Foods",
                newName: "EmployPrice");

            migrationBuilder.RenameColumn(
                name: "TotalEmployeePrice",
                table: "FoodOrders",
                newName: "TotalEmployPrice");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "FoodOrders",
                newName: "EmployeId");

            migrationBuilder.RenameIndex(
                name: "IX_FoodOrders_EmployeeId",
                table: "FoodOrders",
                newName: "IX_FoodOrders_EmployeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Employes_EmployeId",
                table: "FoodOrders",
                column: "EmployeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
