using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class EmployeeFoodOrdersTableDataModel
    {
        public int FoodOrderId { get; set; }
        public string FoodName { get; set; }
        public int FoodType { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int OrderCompleteStatus { get; set; }
        public string EmployeeCode { get; set; }
        public Employee Employee { get; set; }
    }
}
