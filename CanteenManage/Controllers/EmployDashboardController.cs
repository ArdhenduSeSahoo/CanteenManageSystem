using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    //[Authorize(Roles = "Employee")]
    public class EmployDashboardController : Controller
    {

        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        private readonly CartService cartService;
        public EmployDashboardController(FoodListingService foodListing, UtilityServices utilityServices, CartService cartService)
        {
            foodListingService = foodListing;
            this.utilityServices = utilityServices;
            this.cartService = cartService;
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
    }
}
