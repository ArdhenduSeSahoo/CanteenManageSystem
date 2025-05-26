using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class MyOrderViewDataModel : LayoutViewDataModel
    {
        public List<FoodOrder> BreakFastFoodOrders { get; set; }
        public List<FoodOrder> LunchFoodOrders { get; set; }
        public List<FoodOrder> SnaksFoodOrders { get; set; }
        public bool ShowAllOrder { get; set; }
    }
}
