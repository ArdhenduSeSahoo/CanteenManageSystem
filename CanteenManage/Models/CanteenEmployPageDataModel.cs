using CanteenManage.Repo.Models;

namespace CanteenManage.Models
{
    public class CanteenEmployPageDataModel
    {
        public List<CanteenFoodDetailsDTOModel> BreakFastFoodOrders { get; set; }
        public List<CanteenFoodDetailsDTOModel> LunchFoodOrders { get; set; }
        public List<CanteenFoodDetailsDTOModel> SnaksFoodOrders { get; set; }
    }
}
