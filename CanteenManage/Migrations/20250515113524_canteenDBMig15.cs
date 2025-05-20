using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodAvailabilityDay_Foods_FoodId",
                table: "FoodAvailabilityDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodAvailabilityDay",
                table: "FoodAvailabilityDay");

            migrationBuilder.RenameTable(
                name: "FoodAvailabilityDay",
                newName: "FoodAvailabilityDays");

            migrationBuilder.RenameIndex(
                name: "IX_FoodAvailabilityDay_FoodId",
                table: "FoodAvailabilityDays",
                newName: "IX_FoodAvailabilityDays_FoodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodAvailabilityDays",
                table: "FoodAvailabilityDays",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmployFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployFeedbacks_Employes_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployFeedbacks_EmployeeId",
                table: "EmployFeedbacks",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodAvailabilityDays_Foods_FoodId",
                table: "FoodAvailabilityDays",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodAvailabilityDays_Foods_FoodId",
                table: "FoodAvailabilityDays");

            migrationBuilder.DropTable(
                name: "EmployFeedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FoodAvailabilityDays",
                table: "FoodAvailabilityDays");

            migrationBuilder.RenameTable(
                name: "FoodAvailabilityDays",
                newName: "FoodAvailabilityDay");

            migrationBuilder.RenameIndex(
                name: "IX_FoodAvailabilityDays_FoodId",
                table: "FoodAvailabilityDay",
                newName: "IX_FoodAvailabilityDay_FoodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FoodAvailabilityDay",
                table: "FoodAvailabilityDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodAvailabilityDay_Foods_FoodId",
                table: "FoodAvailabilityDay",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
