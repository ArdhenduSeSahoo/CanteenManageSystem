using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class EmployDashboardController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        private readonly FoodListingService foodListingService;
        public EmployDashboardController(FoodListingService foodListing)
        {
            foodListingService = foodListing;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (HttpContext.Session.GetString(SessionConstants.UserId) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
            EmployDashboardViewDataModel employDashboardViewDataModel = new EmployDashboardViewDataModel();
            employDashboardViewDataModel.UserName = sessionDataModel.UserName;
            employDashboardViewDataModel.UserId = sessionDataModel.UserId;
            employDashboardViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(sessionDataModel.UserId ?? 0, cancellationToken);

            return View(employDashboardViewDataModel);
        }
    }
}
