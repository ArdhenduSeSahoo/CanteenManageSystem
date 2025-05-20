using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Models
{
    public class CartItemListPanelViewDataModel
    {
        public string panelTitle { get; set; } = "";
        public List<EmployeeCart> employeeCarts { get; set; }
    }
}
