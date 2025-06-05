using CanteenManage.CanteenRepository.Models;
using System.ComponentModel.DataAnnotations;

namespace CanteenManage.Models
{
    public class FoodFormDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal EmployeePrice { get; set; }
        public decimal SubsidyPrice { get; set; }
        public int FoodTypeId { get; set; }
        public FoodType FoodType { get; set; }
        public IEnumerable<FoodType> AllFoodType { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string ImageUrl { get; set; }
        public double Rating { get; set; }
        public int AvailableOnDay { get; set; }
        public IFormFile? FoodImage { get; set; }

        public bool WeekOneAndFive { get; set; }
        public bool WeekTwo { get; set; }
        public bool WeekThree { get; set; }
        public bool WeekFour { get; set; }
        public bool MonDay { get; set; }
        public bool TuesDay { get; set; }
        public bool WednesDay { get; set; }
        public bool ThusDay { get; set; }
        public bool FriDay { get; set; }

    }
}
