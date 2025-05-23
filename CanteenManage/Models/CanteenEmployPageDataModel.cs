using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class CanteenEmployPageDataModel : LayoutViewDataModel
    {
        public List<CanteenFoodDetailsDTOModel> BreakFastFoodOrders { get; set; }
        public List<CanteenFoodDetailsDTOModel> LunchFoodOrders { get; set; }
        public List<CanteenFoodDetailsDTOModel> SnaksFoodOrders { get; set; }
    }
}
