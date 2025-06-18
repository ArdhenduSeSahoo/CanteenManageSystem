namespace CanteenManage.Models
{
    public class CanteenDashboardViewDataModel
    {
        public int TodaysTotalCount { get; set; }
        public int TodaysCompletedCount { get; set; }
        public int TodaysUnCompletedCount { get; set; }
        public int TomorrowCount { get; set; }
        public int AllCount { get; set; }
        public string? PanelTitle { get; set; }

        public List<CanteenFoodDetailsDTOModel> BreakFastFoodOrders { get; set; } = new List<CanteenFoodDetailsDTOModel>();
        public List<CanteenFoodDetailsDTOModel> LunchFoodOrders { get; set; } = new List<CanteenFoodDetailsDTOModel>();
        public List<CanteenFoodDetailsDTOModel> SnaksFoodOrders { get; set; } = new List<CanteenFoodDetailsDTOModel>();


    }
}
