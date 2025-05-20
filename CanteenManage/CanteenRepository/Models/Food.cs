using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.CanteenRepository.Models
{
    public class Food
    {
        public int Id { get; set; }
        [StringLength(1000)]
        public string Name { get; set; }
        [StringLength(10000)]
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal EmployeePrice { get; set; }
        public decimal SubsidyPrice { get; set; }
        public int FoodTypeId { get; set; }
        public FoodType FoodType { get; set; }
        public bool IsAvailable { get; set; } = true;
        [StringLength(1000)]
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public int AvailableOnDay { get; set; }

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public ICollection<FoodAvailabilityDay> FoodAvailabilityDays { get; set; } = new List<FoodAvailabilityDay>();
        public ICollection<FoodOrder> FoodOrders { get; set; } = new List<FoodOrder>();
        public ICollection<EmployeeCart> EmployeeCarts { get; set; } = new List<EmployeeCart>();
    }
}
