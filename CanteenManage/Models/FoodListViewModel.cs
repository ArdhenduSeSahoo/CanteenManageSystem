using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class FoodListViewModel
    {
        public string SearchTerm { get; set; }
        public List<Food> Foods { get; set; }
    }
}
