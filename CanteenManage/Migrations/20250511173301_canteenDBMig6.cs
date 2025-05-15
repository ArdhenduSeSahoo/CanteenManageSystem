using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Foods",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Foods",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "EmployPrice",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubsidyPrice",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrderCompleteStatusId",
                table: "FoodOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Employes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Employes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Employes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "EmployTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 4, "committee members", "Committee Members" });

            migrationBuilder.UpdateData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "");

            migrationBuilder.UpdateData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "");

            migrationBuilder.UpdateData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "");

            migrationBuilder.UpdateData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "");

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EmployPrice", "ImageUrl", "SubsidyPrice" },
                values: new object[] { 0m, "Egg_Thali.jpg", 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "EmployPrice", "SubsidyPrice" },
                values: new object[] { 0m, 0m });

            migrationBuilder.InsertData(
                table: "Employes",
                columns: new[] { "Id", "Email", "EmployID", "EmployTypeId", "IsActive", "Name", "Password", "PhoneNumber" },
                values: new object[] { 5, "", "EMP005", 4, true, "Ardhendu Member", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EmployTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "EmployPrice",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "SubsidyPrice",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "OrderCompleteStatusId",
                table: "FoodOrders");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Employes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Employes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Employes",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImageUrl",
                value: "Chicken_Thali.jpg");
        }
    }
}
