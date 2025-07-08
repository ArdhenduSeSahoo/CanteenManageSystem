using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models.DTO;

namespace CanteenManage.Models
{
    public class MyOrderViewDataModel : LayoutViewDataModel
    {
        public List<FoodOrderDto> BreakFastFoodOrders { get; set; } = new List<FoodOrderDto>();
        public List<FoodOrderDto> LunchFoodOrders { get; set; } = new List<FoodOrderDto>();
        public List<FoodOrderDto> SnaksFoodOrders { get; set; } = new List<FoodOrderDto>();
        public bool? ShowAllOrder { get; set; } = false;
    }
}
