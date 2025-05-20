using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class BreakFastPageDataModel : LayoutViewDataModel
    {
        public List<DaysOfWeekModel> DayOfWeeks { get; set; }
        public FoodOrder foodOrder { get; set; }
        public int totalCountForSelectedDay { get; set; }
        public List<Food> foods { get; set; }
    }
}
