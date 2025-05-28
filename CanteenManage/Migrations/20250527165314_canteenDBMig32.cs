using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCarts_Employes_EmployeeId",
                table: "EmployeeCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Employes_EmployTypes_EmployTypeId",
                table: "Employes");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployFeedbacks_Employes_EmployeeId",
                table: "EmployFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Employes_EmployeeId",
                table: "FoodOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployTypes",
                table: "EmployTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployFeedbacks",
                table: "EmployFeedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employes",
                table: "Employes");

            migrationBuilder.RenameTable(
                name: "EmployTypes",
                newName: "EmployeeTypes");

            migrationBuilder.RenameTable(
                name: "EmployFeedbacks",
                newName: "EmployeeFeedbacks");

            migrationBuilder.RenameTable(
                name: "Employes",
                newName: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_EmployFeedbacks_EmployeeId",
                table: "EmployeeFeedbacks",
                newName: "IX_EmployeeFeedbacks_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Employes_EmployTypeId",
                table: "Employees",
                newName: "IX_Employees_EmployTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTypes",
                table: "EmployeeTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeFeedbacks",
                table: "EmployeeFeedbacks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCarts_Employees_EmployeeId",
                table: "EmployeeCarts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFeedbacks_Employees_EmployeeId",
                table: "EmployeeFeedbacks",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_EmployeeTypes_EmployTypeId",
                table: "Employees",
                column: "EmployTypeId",
                principalTable: "EmployeeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Employees_EmployeeId",
                table: "FoodOrders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeCarts_Employees_EmployeeId",
                table: "EmployeeCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFeedbacks_Employees_EmployeeId",
                table: "EmployeeFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_EmployeeTypes_EmployTypeId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrders_Employees_EmployeeId",
                table: "FoodOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTypes",
                table: "EmployeeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeFeedbacks",
                table: "EmployeeFeedbacks");

            migrationBuilder.RenameTable(
                name: "EmployeeTypes",
                newName: "EmployTypes");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employes");

            migrationBuilder.RenameTable(
                name: "EmployeeFeedbacks",
                newName: "EmployFeedbacks");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_EmployTypeId",
                table: "Employes",
                newName: "IX_Employes_EmployTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFeedbacks_EmployeeId",
                table: "EmployFeedbacks",
                newName: "IX_EmployFeedbacks_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployTypes",
                table: "EmployTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employes",
                table: "Employes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployFeedbacks",
                table: "EmployFeedbacks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeCarts_Employes_EmployeeId",
                table: "EmployeeCarts",
                column: "EmployeeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employes_EmployTypes_EmployTypeId",
                table: "Employes",
                column: "EmployTypeId",
                principalTable: "EmployTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployFeedbacks_Employes_EmployeeId",
                table: "EmployFeedbacks",
                column: "EmployeeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrders_Employes_EmployeeId",
                table: "FoodOrders",
                column: "EmployeeId",
                principalTable: "Employes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
