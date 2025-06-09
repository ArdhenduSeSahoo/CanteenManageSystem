using CanteenManage.Models;
using System.Threading;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class DashboardController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;

        public DashboardController(FoodListingService foodListing, UtilityServices utilityServices)
        {
            foodListingService = foodListing;
            this.utilityServices = utilityServices;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            var breakfastFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Breakfast, cancellationToken);
            var lunchFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Lunch, cancellationToken);
            var snacksFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Snacks, cancellationToken);

            EmployeeDashboardViewDataModel employeeDashboardViewDataModel = new EmployeeDashboardViewDataModel();
            employeeDashboardViewDataModel.UserName = sessionDataModel.UserName;
            employeeDashboardViewDataModel.UserId = sessionDataModel.UserId;
            employeeDashboardViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(sessionDataModel.UserId ?? 0, cancellationToken);

            employeeDashboardViewDataModel.BreakfastFoods = string.Join(", ", breakfastFoods);
            employeeDashboardViewDataModel.LunchFoods = string.Join(", ", lunchFoods);
            employeeDashboardViewDataModel.SnacksFoods = string.Join(", ", snacksFoods);

            return View(employeeDashboardViewDataModel);
        }
        public async Task<IActionResult> QuickFood(CancellationToken cancellationToken)
        {
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            var data = await foodListingService.GetquickfoodsAsync(cancellationToken);
            EmployeeDashboardViewDataModel employeeDashboardViewDataModel = new EmployeeDashboardViewDataModel();
            employeeDashboardViewDataModel.Foods = data;
            employeeDashboardViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(sessionDataModel.UserId ?? 0, cancellationToken);
            return View(employeeDashboardViewDataModel);
        }
    }
}
