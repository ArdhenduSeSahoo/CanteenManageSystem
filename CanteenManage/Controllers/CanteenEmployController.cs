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
        public CanteenEmployController(FoodListingService foodListingService)
        {
            this.foodListingService = foodListingService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
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
