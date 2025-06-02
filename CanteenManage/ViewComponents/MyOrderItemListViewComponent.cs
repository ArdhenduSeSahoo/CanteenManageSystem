using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.ViewComponents
{
    public class MyOrderItemListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // You can pass any model to the view component if needed
            return View("ItemListView");
        }
    }
}
