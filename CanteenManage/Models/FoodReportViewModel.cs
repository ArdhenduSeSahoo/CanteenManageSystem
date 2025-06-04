using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class FoodReportViewModel : LayoutViewDataModel
    {
        public List<FoodOrder> FoodOrders { get; set; }
    }
}
