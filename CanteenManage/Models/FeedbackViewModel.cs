using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class FeedbackViewModel : LayoutViewDataModel
    {
        public List<FoodOrder> foodOrders { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
