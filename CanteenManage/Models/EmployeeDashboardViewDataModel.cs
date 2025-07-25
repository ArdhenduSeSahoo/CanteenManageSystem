using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class EmployeeDashboardViewDataModel : LayoutViewDataModel
    {
        public string BreakfastFoods { get; set; } = string.Empty;
        public string LunchFoods { get; set; } = string.Empty;
        public string SnacksFoods { get; set; } = string.Empty;
        public string DinnerFoods { get; set; } = string.Empty;
        public string MyOrderBreakfastFoods { get; set; } = string.Empty;
        public string MyOrderLunchFoods { get; set; } = string.Empty;
        public string MyOrderSnacksFoods { get; set; } = string.Empty;
        public string MyOrderDinnerFoods { get; set; } = string.Empty;
        public List<FoodDetails> Foods { get; set; } = new List<FoodDetails>();
    }
}
