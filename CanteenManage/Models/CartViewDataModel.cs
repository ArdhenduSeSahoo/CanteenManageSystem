using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class CartViewDataModel : LayoutViewDataModel
    {
        public List<EmployeeCart> BreakFastFoodOrders { get; set; }
        public List<EmployeeCart> LunchFoodOrders { get; set; }
        public List<EmployeeCart> SnaksFoodOrders { get; set; }
        public List<EmployeeCart> OutOfStockOrders { get; set; }
    }
}
