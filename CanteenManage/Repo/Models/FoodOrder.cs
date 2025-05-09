namespace CanteenManage.Repo.Models
{
    public class FoodOrder
    {
        public int Id { get; set; }
        public int EmployeId { get; set; }
        public Employe Employe { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public Food Food { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int? Rating { get; set; }
        public string Review { get; set; } = string.Empty;
        public DateTime RatingCreatedAt { get; set; }
    }
}
