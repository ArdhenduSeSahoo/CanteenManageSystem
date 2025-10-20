using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authorization;
using CanteenManage.Models.DTO;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class OrderHistoryController : Controller
    {

        private readonly OrderingService orderingService;
        private readonly UtilityServices utilityServices;
        private readonly ILogger<OrderHistoryController> logger;
        public OrderHistoryController(OrderingService ordering, UtilityServices utilityServices, ILogger<OrderHistoryController> logger)
        {

            this.orderingService = ordering;
            this.utilityServices = utilityServices;
            this.logger = logger;
        }
        public async Task<IActionResult> Index()
        {

            OrderHistoryPageDataModel myOrderViewDataModel = new OrderHistoryPageDataModel();
            myOrderViewDataModel.BreakFastFoodOrders = new List<FoodOrderDto>();
            myOrderViewDataModel.LunchFoodOrders = new List<FoodOrderDto>();
            myOrderViewDataModel.SnaksFoodOrders = new List<FoodOrderDto>();
            myOrderViewDataModel.DinnerFoodOrders = new List<FoodOrderDto>();
            try
            {

                myOrderViewDataModel.SnaksFoodOrders = await orderingService.getOrderHistoryList((int)FoodTypeEnum.Snacks,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.LunchFoodOrders = await orderingService.getOrderHistoryList((int)FoodTypeEnum.Lunch,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.BreakFastFoodOrders = await orderingService.getOrderHistoryList((int)FoodTypeEnum.Breakfast,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.DinnerFoodOrders = await orderingService.getOrderHistoryList((int)FoodTypeEnum.Dinner,
                    utilityServices.getSessionUserId(HttpContext.Session)
                    );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "orderHistoryControler");
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
                    await orderingService.addReview(sessionDataModel, (orderId), int.Parse(options), review);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "orderHistoryControler add review");
            }
            return RedirectToAction("Index");
        }


    }
}
