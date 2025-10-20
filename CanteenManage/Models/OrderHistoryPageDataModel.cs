using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models.DTO;

namespace CanteenManage.Models
{
    public class OrderHistoryPageDataModel : LayoutViewDataModel
    {
        public List<FoodOrderDto> BreakFastFoodOrders { get; set; }
        public List<FoodOrderDto> LunchFoodOrders { get; set; }
        public List<FoodOrderDto> SnaksFoodOrders { get; set; }
        public List<FoodOrderDto> DinnerFoodOrders { get; set; }
    }
}
