using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class addedFoodOrderFoodDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "FoodOrders",
                newName: "IsCompleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "FoodOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "FoodOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "FoodOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoodOrderFoodDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodOrderId = table.Column<int>(type: "int", nullable: true),
                    FoodOrder_OrderID = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FoodId = table.Column<int>(type: "int", nullable: true),
                    FoodName = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    EmployeeEId = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalEmployeePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSubsidyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    RatingCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Review = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ActionTaken = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodOrderFoodDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodOrderFoodDetail_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FoodOrderFoodDetail_FoodOrders_FoodOrderId",
                        column: x => x.FoodOrderId,
                        principalTable: "FoodOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FoodOrderFoodDetail_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrderFoodDetail_EmployeeId",
                table: "FoodOrderFoodDetail",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrderFoodDetail_FoodId",
                table: "FoodOrderFoodDetail",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrderFoodDetail_FoodOrderId",
                table: "FoodOrderFoodDetail",
                column: "FoodOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodOrderFoodDetail");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "FoodOrders");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "FoodOrders");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "FoodOrders");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "FoodOrders",
                newName: "IsDeleted");
        }
    }
}
