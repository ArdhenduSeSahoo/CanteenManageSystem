using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class CartViewDataModel : LayoutViewDataModel
    {
        public List<EmployeeCart> BreakFastFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> LunchFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> SnaksFoodOrders { get; set; } = new List<EmployeeCart>();
        public List<EmployeeCart> OutOfStockOrders { get; set; } = new List<EmployeeCart>();
        public List<CartItemInOrder> CartItemInOrders { get; set; } = new List<CartItemInOrder>();
    }

    public class CartItemInOrder
    {
        public string ItemName { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
