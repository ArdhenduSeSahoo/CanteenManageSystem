using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class WeeklyFoodViewModel
    {
        
        public List<WeeklyFoodList> weekly1_FoodLists { get; set; } = new List<WeeklyFoodList>();
        public List<WeeklyFoodList> weekly2_FoodLists { get; set; } = new List<WeeklyFoodList>();
        public List<WeeklyFoodList> weekly3_FoodLists { get; set; } = new List<WeeklyFoodList>();
        public List<WeeklyFoodList> weekly4_FoodLists { get; set; } = new List<WeeklyFoodList>();
        public List<WeeklyFoodList> weekly5_FoodLists { get; set; } = new List<WeeklyFoodList>();
        
    }
    public class WeeklyFoodList
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public List<Food> Foods { get; set; } = new List<Food>();
    }
}
