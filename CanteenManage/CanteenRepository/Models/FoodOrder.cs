using System.ComponentModel.DataAnnotations;

namespace CanteenManage.CanteenRepository.Models
{
    public class FoodOrder
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public virtual Food? Food { get; set; }
        public int? FoodId { get; set; }
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalEmployeePrice { get; set; }
        public decimal TotalSubsidyPrice { get; set; }
        public int OrderCompleteStatus { get; set; }
        public int? Rating { get; set; }
        [StringLength(10000)]
        public string Review { get; set; } = string.Empty;
        public DateTime RatingCreatedAt { get; set; }


    }
}
