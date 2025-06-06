using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class EmployeeDashboardViewDataModel : LayoutViewDataModel
    {
        public string BreakfastFoods { get; set; }
        public string LunchFoods { get; set; }
        public string SnacksFoods { get; set; }
        public List<Food> Foods { get; set; }
    }
}
