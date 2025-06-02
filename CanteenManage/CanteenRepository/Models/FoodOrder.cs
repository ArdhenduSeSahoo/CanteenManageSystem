using System.ComponentModel.DataAnnotations;

namespace CanteenManage.CanteenRepository.Models
{
    public class FoodOrder
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderDateCustom { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public int? FoodId { get; set; }
        public Food? Food { get; set; }

        [StringLength(5000)]
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalEmployeePrice { get; set; }
        public decimal TotalSubsidyPrice { get; set; }
        public int OrderStatus { get; set; }
        public int? Rating { get; set; }
        [StringLength(10000)]
        public string Review { get; set; } = string.Empty;
        public string ActionTaken { get; set; } = string.Empty;
        public DateTime RatingCreatedAt { get; set; }
        [StringLength(1000)]
        public required string OrderID { get; set; }
        public int OrderSerialNumber { get; set; }
        public required string OrderPlacedID { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public bool IsCanceled { get; set; } = false;
        public DateTime? CanceledAt { get; set; }
        //public ICollection<FoodOrderFoodDetail> FoodOrderFoodDetails { get; set; } = new List<FoodOrderFoodDetail>();
    }
}
