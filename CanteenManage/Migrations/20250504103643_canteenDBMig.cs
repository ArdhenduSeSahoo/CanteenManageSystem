using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployID",
                table: "Employes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Employes",
                columns: new[] { "Id", "Email", "EmployID", "EmployTypeId", "IsActive", "Name", "PhoneNumber" },
                values: new object[] { 1, "", "EMP001", 3, true, "Ardhendu Sekhar Sahoo", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "EmployID",
                table: "Employes");
        }
    }
}
