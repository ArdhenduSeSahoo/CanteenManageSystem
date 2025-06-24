namespace CanteenManage.Models
{
    public class OrderDetailDto: LayoutViewDataModel
    {
        public string? MealType { get; set; }
        public List<OrderItemDto> Orders { get; set; } = new();
    }
    public class OrderItemDto
    {
        public string OrderID { get; set; }
        public int? EmployeeId { get; set; }
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
    }
}
