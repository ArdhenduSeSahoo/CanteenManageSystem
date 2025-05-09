namespace CanteenManage.Repo.Models
{
    public class FoodReviewDetails
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
        public int TotalRating { get; set; }
        public int TotalUserCount { get; set; }
    }
}
