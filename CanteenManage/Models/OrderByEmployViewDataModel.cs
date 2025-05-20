namespace CanteenManage.Models
{
    public class OrderByEmployViewDataModel : LayoutViewDataModel
    {
        public string screenTitle { get; set; } = string.Empty;
        public List<EmployFoodOrdersTableDataModel> FoodOrders { get; set; } = new List<EmployFoodOrdersTableDataModel>();
        public int FoodType { get; set; } = 1;

    }
}
