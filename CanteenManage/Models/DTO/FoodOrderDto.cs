using CanteenManage.CanteenRepository.Models;
using System.ComponentModel.DataAnnotations;

namespace CanteenManage.Models.DTO
{
    public class FoodOrderDto
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderDateCustom { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public int? FoodId { get; set; }
        public Food? Food { get; set; }

        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalEmployeePrice { get; set; }
        public decimal TotalSubsidyPrice { get; set; }
        public int? Rating { get; set; }
        public string Review { get; set; } = string.Empty;
        public string ActionTaken { get; set; } = string.Empty;

        public string OrderID { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public bool IsCanceled { get; set; } = false;
        public DateTime? CanceledAt { get; set; }

        public string QrCodeText {  get; set; }= string.Empty;
    }
}
