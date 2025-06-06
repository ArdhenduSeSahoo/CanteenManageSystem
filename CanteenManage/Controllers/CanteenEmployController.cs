
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class CanteenEmployController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        public CanteenEmployController(FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            var snackesFoodlist = await foodListingService.getCanteenUserFoodOrderGroupList(3, cancellationToken);

            var lunchFoodlist = await foodListingService.getCanteenUserFoodOrderGroupList(2, cancellationToken);
            var breakfastFoodlist = await foodListingService.getCanteenUserFoodOrderGroupList(1, cancellationToken);

            CanteenEmployPageDataModel canteenEmployPageDataModel = new CanteenEmployPageDataModel();
            canteenEmployPageDataModel.SnaksFoodOrders = snackesFoodlist;
            canteenEmployPageDataModel.LunchFoodOrders = lunchFoodlist;
            canteenEmployPageDataModel.BreakFastFoodOrders = breakfastFoodlist;
            return View(canteenEmployPageDataModel);
        }
    }
}
