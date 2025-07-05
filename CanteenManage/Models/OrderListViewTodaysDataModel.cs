namespace CanteenManage.Models
{
    public class OrderListViewTodaysDataModel
    {
        public List<EmployeeFoodOrdersTableDataModel> EmployeeFoodOrdersTableData { get; set; } = new List<EmployeeFoodOrdersTableDataModel>();
        public int FoodType { get; set; } = 1;
    }
}
