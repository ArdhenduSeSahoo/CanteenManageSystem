using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class CartViewDataModel : LayoutViewDataModel
    {
        public List<EmployeeCart> BreakFastFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> LunchFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> SnacksFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> DinnerFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> OutOfStockOrders { get; set; } = new List<EmployeeCart>();
        public List<CartItemInOrder> CartItemInOrders { get; set; } = new List<CartItemInOrder>();
        public List<CartItemInOrder> MaxCartItemInOrders { get; set; } = new List<CartItemInOrder>();
    }

    public class CartItemInOrder
    {
        public string ItemName { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public Food? Food { get; set; }
        public List<FoodOrder?> FoodOrders { get; set; }
    }
}
