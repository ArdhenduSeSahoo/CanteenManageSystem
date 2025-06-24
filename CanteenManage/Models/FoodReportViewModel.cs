using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class FoodReportViewModel : LayoutViewDataModel
    {
        public List<FoodOrder> FoodOrders { get; set; }
        public string ReportDate { get; set; }
        public List<FoodReportDetailsViewModel> FoodOrdersDetails { get; set; }
    }

    public class FoodReportDetailsViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string FoodName { get; set; } = string.Empty;
        public int FoodTypeid { get; set; }
        public string FoodTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal EmployeePrice { get; set; }
        public decimal SubsidiaryPrice { get; set; }
    }
}
