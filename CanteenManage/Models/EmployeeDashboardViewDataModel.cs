using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class EmployeeDashboardViewDataModel : LayoutViewDataModel
    {
        public string BreakfastFoods { get; set; } = string.Empty;
        public string LunchFoods { get; set; } = string.Empty;
        public string SnacksFoods { get; set; } = string.Empty;
        public List<FoodDetails> Foods { get; set; } = new List<FoodDetails>();
    }
}
