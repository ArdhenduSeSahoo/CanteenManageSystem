using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CanteenManage.Migrations
{
    /// <inheritdoc />
    public partial class canteenMigration_seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmployTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Admin", "Admin" },
                    { 2, "CanteenStaf", "CanteenStaf" },
                    { 3, "Employee", "Employee" }
                });

            migrationBuilder.InsertData(
                table: "FoodTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Breakfast" },
                    { 2, "Lunch" },
                    { 3, "Evening Snacks" }
                });

            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "Description", "FoodTypeId", "ImageUrl", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "", 1, "Veg_Sandwich.jpg", true, "Veg Sandwich", 60m },
                    { 2, "Dosa with Sambar & Chutney", 1, "Masala_Dosa.jpg", true, "Masala Dosa", 80m },
                    { 3, "Paratha with Curd & Pickle", 1, "Paratha.jpg", true, "Paratha", 50m },
                    { 4, "Poha with Sev & Lemon", 1, "Poha.jpg", true, "Poha", 40m },
                    { 5, "Idli (4 pcs) with Sambar & Chutney", 1, "Idli.jpg", true, "Idli", 40m },
                    { 6, "Aloo Puri (4 puris)", 1, "Aloo Puri.jpg", true, "Aloo Puri", 40m },
                    { 7, "", 1, "Bread_Omelette.jpg", true, "Bread & Omelette", 40m },
                    { 8, "", 1, "Chai_Tea.jpg", true, "Chai (Tea)", 15m },
                    { 9, "", 2, "Veg_Thali.jpg", true, "Veg Thali", 120m },
                    { 10, "", 2, "Chole_Chawal.jpg", true, "Chole Chawal", 140m },
                    { 11, "Veg Biryani with Raita", 2, "Veg_Biryani.jpg", true, "Veg Biryani", 110m },
                    { 12, "", 2, "Rajma_Chawal.jpg", true, "Rajma Chawal", 90m },
                    { 13, "", 2, "Chicken_Thali.jpg", true, "Chicken Thali", 160m },
                    { 14, "Egg Curry with Rice or 2 Rotis", 2, "Chicken_Thali.jpg", true, "Egg Curry with Rice", 100m },
                    { 15, "Chicken Biryani with Raita", 2, "Chicken_Biryani.jpg", true, "Chicken Biryani", 150m },
                    { 16, "", 2, "Fish_Curry_Rice.jpg", true, "Fish Curry with Rice", 170m },
                    { 17, "", 2, "Roti.jpg", true, "Roti", 10m },
                    { 18, "", 2, "Curd.jpg", true, "Curd", 20m },
                    { 19, "", 2, "Salad.jpg", true, "Salad", 15m },
                    { 20, "Papad (Roasted/Fried)", 2, "Papad.jpg", true, "Papad", 10m },
                    { 21, "Samosa (2 pcs)", 3, "Samosa.jpg", true, "Samosa", 30m },
                    { 22, "", 3, "Veg_Puff.jpg", true, "Veg Puff", 25m },
                    { 23, "Bread Pakora (2 pcs)", 3, "Bread_Pakora.jpg", true, "Bread Pakora", 40m },
                    { 24, "", 3, "Onion_Pakoda.jpg", true, "Onion Pakoda", 45m },
                    { 25, "Aloo Tikki (2 pcs)", 3, "Aloo_Tikki.jpg", true, "Aloo Tikki", 40m },
                    { 26, "", 3, "Maggi_Noodles.jpg", true, "Maggi Noodles", 30m },
                    { 27, "", 3, "Pav_Bhaji.jpg", true, "Pav Bhaji", 70m },
                    { 28, "", 3, "Roll_Veg.jpg", true, "Roll (Veg)", 40m },
                    { 29, "", 3, "Non_Veg_Roll.jpg", true, "Non-Veg Roll", 50m },
                    { 30, "", 3, "Chai.jpg", true, "Chai (Tea)", 15m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmployTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmployTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmployTypes",
                keyColumn: "Id",
                keyValue: 3);

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

            migrationBuilder.DeleteData(
                table: "FoodTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FoodTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FoodTypes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
