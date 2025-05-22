using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenDBMig23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 1);

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

            migrationBuilder.DeleteData(
                table: "Employes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "ActionTaken",
                table: "FoodOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionTaken",
                table: "FoodOrders");

            migrationBuilder.InsertData(
                table: "Employes",
                columns: new[] { "Id", "Email", "EmployID", "EmployTypeId", "IsActive", "Name", "Password", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "", "SD1265", 3, true, "Ardhendu Sekhar Sahoo", "", "" },
                    { 2, "", "EMP002", 3, true, "Rojalin ", "", "" },
                    { 3, "", "EMP003", 2, true, "Satyajit", "", "" },
                    { 4, "", "EMP004", 1, true, "Ardhendu Admin", "", "" },
                    { 5, "", "EMP005", 4, true, "Ardhendu Member", "", "" }
                });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "AvailableOnDay", "Description", "EmployeePrice", "FoodTypeId", "ImageUrl", "IsAvailable", "Name", "Price", "Rating", "SubsidyPrice" },
                values: new object[,]
                {
                    { 1, 0, "", 0m, 1, "Veg_Sandwich.jpg", true, "Veg Sandwich", 60m, 0.0, 0m },
                    { 2, 0, "Dosa with Sambar & Chutney", 0m, 1, "Masala_Dosa.jpg", true, "Masala Dosa", 80m, 0.0, 0m },
                    { 3, 0, "Paratha with Curd & Pickle", 0m, 1, "Paratha.jpg", true, "Paratha", 50m, 0.0, 0m },
                    { 4, 0, "Poha with Sev & Lemon", 0m, 1, "Poha.jpg", true, "Poha", 40m, 0.0, 0m },
                    { 5, 0, "Idli (4 pcs) with Sambar & Chutney", 0m, 1, "Idli.jpg", true, "Idli", 40m, 0.0, 0m },
                    { 6, 0, "Aloo Puri (4 puris)", 0m, 1, "Aloo Puri.jpg", true, "Aloo Puri", 40m, 0.0, 0m },
                    { 7, 0, "", 0m, 1, "Bread_Omelette.jpg", true, "Bread & Omelette", 40m, 0.0, 0m },
                    { 8, 0, "", 0m, 1, "Chai_Tea.jpg", true, "Chai (Tea)", 15m, 0.0, 0m },
                    { 9, 0, "", 0m, 2, "Veg_Thali.jpg", true, "Veg Thali", 120m, 0.0, 0m },
                    { 10, 0, "", 0m, 2, "Chole_Chawal.jpg", true, "Chole Chawal", 140m, 0.0, 0m },
                    { 11, 0, "Veg Biryani with Raita", 0m, 2, "Veg_Biryani.jpg", true, "Veg Biryani", 110m, 0.0, 0m },
                    { 12, 0, "", 0m, 2, "Rajma_Chawal.jpg", true, "Rajma Chawal", 90m, 0.0, 0m },
                    { 13, 0, "", 0m, 2, "Chicken_Thali.jpg", true, "Chicken Thali", 160m, 0.0, 0m },
                    { 14, 0, "Egg Curry with Rice or 2 Rotis", 0m, 2, "Egg_Thali.jpg", true, "Egg Curry with Rice", 100m, 0.0, 0m },
                    { 15, 0, "Chicken Biryani with Raita", 0m, 2, "Chicken_Biryani.jpg", true, "Chicken Biryani", 150m, 0.0, 0m },
                    { 16, 0, "", 0m, 2, "Fish_Curry_Rice.jpg", true, "Fish Curry with Rice", 170m, 0.0, 0m },
                    { 17, 0, "", 0m, 2, "Roti.jpg", true, "Roti", 10m, 0.0, 0m },
                    { 18, 0, "", 0m, 2, "Curd.jpg", true, "Curd", 20m, 0.0, 0m },
                    { 19, 0, "", 0m, 2, "Salad.jpg", true, "Salad", 15m, 0.0, 0m },
                    { 20, 0, "Papad (Roasted/Fried)", 0m, 2, "Papad.jpg", true, "Papad", 10m, 0.0, 0m },
                    { 21, 0, "Samosa (2 pcs)", 0m, 3, "Samosa.jpg", true, "Samosa", 30m, 0.0, 0m },
                    { 22, 0, "", 0m, 3, "Veg_Puff.jpg", true, "Veg Puff", 25m, 0.0, 0m },
                    { 23, 0, "Bread Pakora (2 pcs)", 0m, 3, "Bread_Pakora.jpg", true, "Bread Pakora", 40m, 0.0, 0m },
                    { 24, 0, "", 0m, 3, "Onion_Pakoda.jpg", true, "Onion Pakoda", 45m, 0.0, 0m },
                    { 25, 0, "Aloo Tikki (2 pcs)", 0m, 3, "Aloo_Tikki.jpg", true, "Aloo Tikki", 40m, 0.0, 0m },
                    { 26, 0, "", 0m, 3, "Maggi_Noodles.jpg", true, "Maggi Noodles", 30m, 0.0, 0m },
                    { 27, 0, "", 0m, 3, "Pav_Bhaji.jpg", true, "Pav Bhaji", 70m, 0.0, 0m },
                    { 28, 0, "", 0m, 3, "Roll_Veg.jpg", true, "Roll (Veg)", 40m, 0.0, 0m },
                    { 29, 0, "", 0m, 3, "Non_Veg_Roll.jpg", true, "Non-Veg Roll", 50m, 0.0, 0m },
                    { 30, 0, "", 0m, 3, "Chai.jpg", true, "Chai (Tea)", 15m, 0.0, 0m }
                });
        }
    }
}
