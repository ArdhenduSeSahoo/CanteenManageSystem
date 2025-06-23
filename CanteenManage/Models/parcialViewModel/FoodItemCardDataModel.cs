using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models.parcialViewModel
{
    public class FoodItemCardDataModel
    {
        public FoodDetails Food { get; set; }
        public bool ShowAddToCartButton { get; set; } = true;
    }
}
