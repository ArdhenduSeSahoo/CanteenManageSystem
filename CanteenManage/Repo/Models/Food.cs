namespace CanteenManage.Repo.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int FoodTypeId { get; set; }
        public FoodType FoodType { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string ImageUrl { get; set; }
        public double Rating { get; set; }
        public ICollection<FoodOrder> FoodOrders { get; set; } = new List<FoodOrder>();


    }
}
