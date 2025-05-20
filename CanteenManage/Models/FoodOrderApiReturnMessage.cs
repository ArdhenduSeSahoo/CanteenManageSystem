namespace CanteenManage.Models
{
    public class FoodOrderApiReturnMessage
    {
        public int food_quantity { get; set; }
        public int total_quantity { get; set; }
        public int total_quantity_cart { get; set; }
        public string message { get; set; } = "";
        public string error { get; set; } = "";
    }
}
