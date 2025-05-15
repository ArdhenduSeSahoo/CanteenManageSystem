namespace CanteenManage.CanteenRepository.Models
{
    public class FoodAvailabilityDay
    {
        public int Id { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
        public int WeekOfMonth { get; set; } // 0 = First week, 1 = Second week, ..., 3 = Fourth week
        public int FoodId { get; set; }
        public Food Food { get; set; }

    }
}
