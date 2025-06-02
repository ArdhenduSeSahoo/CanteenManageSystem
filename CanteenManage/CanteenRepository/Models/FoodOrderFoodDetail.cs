using System.ComponentModel.DataAnnotations;

namespace CanteenManage.CanteenRepository.Models
{
    public class FoodOrderFoodDetail
    {
        public int Id { get; set; }
        public int? FoodOrderId { get; set; }
        public FoodOrder? FoodOrder { get; set; }
        [StringLength(1000)]
        public string? FoodOrder_OrderID { get; set; } = string.Empty;
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        [StringLength(5000)]
        public string? FoodName { get; set; }
        public int? FoodTypeId { get; set; }
        public FoodType? FoodType { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        [StringLength(1000)]
        public string? EmployeeEId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public DateTime OrderDateCustom { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalEmployeePrice { get; set; }
        public decimal TotalSubsidyPrice { get; set; }
        public bool? IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public bool IsCanceled { get; set; } = false;
        public DateTime? CanceledAt { get; set; }

        public int? Rating { get; set; }
        public DateTime? RatingCreatedAt { get; set; }
        [StringLength(5000)]
        public string? Review { get; set; } = string.Empty;
        [StringLength(20000)]
        public string? ActionTaken { get; set; } = string.Empty;
    }
}
