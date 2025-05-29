using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;

namespace CanteenManage.Models
{
    public class MyOrderListPanelViewModel
    {
        public string PanelTitle { get; set; }
        public List<FoodOrderFoodDetail> FoodOrders { get; set; }
        public FoodTypeEnum FoodType { get; set; }
    }
}
