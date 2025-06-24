using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class MyOrderViewDataModel : LayoutViewDataModel
    {
        public List<FoodOrder> BreakFastFoodOrders { get; set; } = new List<FoodOrder>();
        public List<FoodOrder> LunchFoodOrders { get; set; } = new List<FoodOrder>();
        public List<FoodOrder> SnaksFoodOrders { get; set; } = new List<FoodOrder>();
        public bool? ShowAllOrder { get; set; } = false;
    }
}
