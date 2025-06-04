using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class OrderHistoryController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        private readonly UtilityServices utilityServices;
        public OrderHistoryController(CanteenManageDBContext canteenManageContext, OrderingService ordering, UtilityServices utilityServices)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = ordering;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> Index()
        {
            //if (utilityServices.getSessionUserId(HttpContext.Session) is null)
            //{
            //    return RedirectToAction("Login", "Index");
            //}
            OrderHistoryPageDataModel myOrderViewDataModel = new OrderHistoryPageDataModel();
            try
            {

                myOrderViewDataModel.SnaksFoodOrders = await orderingService.getOrderHistoryList(3,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.LunchFoodOrders = await orderingService.getOrderHistoryList(2,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.BreakFastFoodOrders = await orderingService.getOrderHistoryList(1,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(myOrderViewDataModel);
        }
        [HttpPost]
        public async Task<IActionResult> addReview(IFormCollection formcollect)
        {

            try
            {
                var options = formcollect["options"];
                var review = formcollect["review_text"];
                var orderId = formcollect["order_id"];
                var reviewdata = formcollect["review_text"];
                if (!string.IsNullOrEmpty(options) || !string.IsNullOrEmpty(review) || !string.IsNullOrEmpty(orderId))
                {
                    SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                    await orderingService.addReview(sessionDataModel, int.Parse(orderId), int.Parse(options), review);
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }


    }
}
