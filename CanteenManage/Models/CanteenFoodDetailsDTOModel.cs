namespace CanteenManage.Models
{
    public class CanteenFoodDetailsDTOModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FoodTypeId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Price { get; set; }
        public int FoodQuantity { get; set; }
        public int EmployId { get; set; }
        public string EmployName { get; set; }

    }
}
