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
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        public CanteenEmployController(CanteenManageDBContext canteenManageContext, OrderingService orderingService)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = orderingService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }

            var snackesFoodlist = await orderingService.getFoodOrderGroupList(3, cancellationToken);

            var lunchFoodlist = await orderingService.getFoodOrderGroupList(2, cancellationToken);
            var breakfastFoodlist = await orderingService.getFoodOrderGroupList(1, cancellationToken);

            CanteenEmployPageDataModel canteenEmployPageDataModel = new CanteenEmployPageDataModel();
            canteenEmployPageDataModel.SnaksFoodOrders = snackesFoodlist;
            canteenEmployPageDataModel.LunchFoodOrders = lunchFoodlist;
            canteenEmployPageDataModel.BreakFastFoodOrders = breakfastFoodlist;
            return View(canteenEmployPageDataModel);
        }
    }
}
