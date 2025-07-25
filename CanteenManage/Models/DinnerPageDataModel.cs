namespace CanteenManage.Models
{
    public class DinnerPageDataModel : LayoutViewDataModel
    {
        public List<DaysOfWeekModel> DayOfWeeks { get; set; }
        public int totalCountForSelectedDay { get; set; }
        public List<FoodDetails> foods { get; set; }
        public bool showAddBtn { get; set; } = false;
    }
}
