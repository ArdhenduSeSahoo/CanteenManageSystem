using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employes",
                columns: new[] { "Id", "Email", "EmployID", "EmployTypeId", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 2, "", "EMP002", 3, true, "Rojalin ", "" },
                    { 3, "", "EMP003", 2, true, "Satyajit", "" },
                    { 4, "", "EMP004", 1, true, "Ardhendu Admin", "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
