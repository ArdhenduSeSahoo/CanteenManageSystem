using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
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
            if (utilityServices.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }

            var snackesFoodlist = await foodListingService.getFoodOrderGroupList(3, cancellationToken);

            var lunchFoodlist = await foodListingService.getFoodOrderGroupList(2, cancellationToken);
            var breakfastFoodlist = await foodListingService.getFoodOrderGroupList(1, cancellationToken);

            CanteenEmployPageDataModel canteenEmployPageDataModel = new CanteenEmployPageDataModel();
            canteenEmployPageDataModel.SnaksFoodOrders = snackesFoodlist;
            canteenEmployPageDataModel.LunchFoodOrders = lunchFoodlist;
            canteenEmployPageDataModel.BreakFastFoodOrders = breakfastFoodlist;
            return View(canteenEmployPageDataModel);
        }
    }
}
