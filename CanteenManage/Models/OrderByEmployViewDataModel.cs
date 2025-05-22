namespace CanteenManage.Models
{
    public class OrderByEmployViewDataModel : LayoutViewDataModel
    {
        public string screenTitle { get; set; } = string.Empty;
        public List<EmployeeFoodOrdersTableDataModel> FoodOrders { get; set; } = new List<EmployeeFoodOrdersTableDataModel>();
        public int FoodType { get; set; } = 1;

    }
}
