using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Repo.Contexts;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
    public class CanteenEmployController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        public CanteenEmployController(CanteenManageDBContext canteenManageContext, OrderingService orderingService)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = orderingService;
        }
        public async Task<IActionResult> Index()
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }

            var snackesFoodlist = await orderingService.getFoodOrderGroupList(3);

            var lunchFoodlist = await orderingService.getFoodOrderGroupList(2);
            var breakfastFoodlist = await orderingService.getFoodOrderGroupList(1);

            CanteenEmployPageDataModel canteenEmployPageDataModel = new CanteenEmployPageDataModel();
            canteenEmployPageDataModel.SnaksFoodOrders = snackesFoodlist;
            canteenEmployPageDataModel.LunchFoodOrders = lunchFoodlist;
            canteenEmployPageDataModel.BreakFastFoodOrders = breakfastFoodlist;
            return View(canteenEmployPageDataModel);
        }
    }
}
