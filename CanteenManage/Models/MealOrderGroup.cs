namespace CanteenManage.Models
{
    public class MealOrderGroup
    {
        public string MealType { get; set; } = string.Empty;
        public List<MealOrderItem> Orders { get; set; } = new();
    }
    public class MealOrderItem
    {
        public string OrderID { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
    }
}
