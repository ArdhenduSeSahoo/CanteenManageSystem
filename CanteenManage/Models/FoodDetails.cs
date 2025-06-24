using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class FoodDetails
    {
        public Food Food { get; set; }
        public int FoodCountInCart { get; set; }
    }
}
