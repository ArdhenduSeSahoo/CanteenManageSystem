using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class OrderHistoryPageDataModel
    {
        public List<FoodOrder> BreakFastFoodOrders { get; set; }
        public List<FoodOrder> LunchFoodOrders { get; set; }
        public List<FoodOrder> SnaksFoodOrders { get; set; }
    }
}
