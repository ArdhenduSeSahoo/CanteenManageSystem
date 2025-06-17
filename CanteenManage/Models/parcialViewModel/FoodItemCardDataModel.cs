using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models.parcialViewModel
{
    public class FoodItemCardDataModel
    {
        public Food Food { get; set; }
        public bool ShowAddToCartButton { get; set; } = true;
    }
}
