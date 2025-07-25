using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authorization;
using CanteenManage.CanteenRepository.Models;
using System.Threading;
using CanteenManage.Models.DTO;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class MyOrdersController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        private readonly UtilityServices utilityServices;
        private readonly FoodListingService foodListingService;
        private readonly OrderingService orderingService;
        private readonly ILogger<MyOrdersController> logger;
        private readonly OrderDataCaching orderDataCaching;

        public MyOrdersController(UtilityServices utility, FoodListingService foodListingService, OrderingService orderingService, ILogger<MyOrdersController> logger, OrderDataCaching orderDataCaching)
        {
            //this.canteenManageContext = canteenManageContext;
            this.utilityServices = utility;
            this.foodListingService = foodListingService;
            this.orderingService = orderingService;
            this.logger = logger;
            this.orderDataCaching = orderDataCaching;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken, bool? ShowAllOrder = null)
        {

            MyOrderViewDataModel myOrderViewDataModel = new MyOrderViewDataModel();
            //myOrderViewDataModel.ShowAllOrder = ShowAllOrder;
            try
            {

                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                //DateTime snacks_dateTime = DateTime.Now.Date;

                List<FoodOrderDto> _snacksorders = new List<FoodOrderDto>();
                List<FoodOrderDto> _dinnerorders = new List<FoodOrderDto>();
                List<FoodOrderDto> _lunchorders = new List<FoodOrderDto>();
                List<FoodOrderDto> _breakfastorders = new List<FoodOrderDto>();
                bool cameFromOtherPage = false;
                if (ShowAllOrder == null)
                {
                    ShowAllOrder = false;
                    cameFromOtherPage = true;
                }

                if (ShowAllOrder ?? false)
                {
                    _snacksorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Snacks,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    _snacksorders = await foodListingService.GetFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Snacks,
                                                                    cancellationToken
                                                                    );

                }

                //////////////////////////////////////////////////////////////

                if (ShowAllOrder ?? false)
                {
                    _lunchorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Lunch,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    _lunchorders = await foodListingService.GetFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Lunch,
                                                                    cancellationToken
                                                                    );
                }

                ////////////////////////////////////////////////////////////
                if (ShowAllOrder ?? false)
                {
                    _breakfastorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Breakfast,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    _breakfastorders = await foodListingService.GetFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Breakfast,
                                                                    cancellationToken
                                                                    );
                }
                ////////////////////////////////////////////////////////////
                if (ShowAllOrder ?? false)
                {
                    _dinnerorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Dinner,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    _dinnerorders = await foodListingService.GetFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Dinner,
                                                                    cancellationToken
                                                                    );
                }
                //if today order are not available then get all orders
                if ((_snacksorders.Count == 0 && _breakfastorders.Count == 0 && _lunchorders.Count == 0) && cameFromOtherPage)
                {
                    _snacksorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Snacks,
                                                                    cancellationToken
                                                                    );
                    _lunchorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Lunch,
                                                                cancellationToken
                                                                );
                    _breakfastorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Breakfast,
                                                                cancellationToken
                                                                );
                    _dinnerorders = await foodListingService.GetFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Dinner,
                                                                cancellationToken
                                                                );
                    ShowAllOrder = true;
                }
                myOrderViewDataModel.ShowAllOrder = ShowAllOrder;
                myOrderViewDataModel.BreakFastFoodOrders = _breakfastorders;
                myOrderViewDataModel.SnacksFoodOrders = _snacksorders;
                myOrderViewDataModel.LunchFoodOrders = _lunchorders;
                myOrderViewDataModel.DinnerFoodOrders = _dinnerorders;
                myOrderViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                           utilityServices.getSessionUserId(HttpContext.Session) ?? 0,
                                                           cancellationToken
                                                           );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(myOrderViewDataModel);
        }


        [HttpPost]
        public async Task<IResult> cancelOrder([FromBody] MyOrderApiDTO myOrderApiDTO, CancellationToken cancellationToken)
        {
            //var req = await HttpContext.Request.Bod();
            var result = new { isDeleted = "no" };
            try
            {
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                await orderingService.RemoveFoodOrder(
                   myOrderApiDTO.orderid,
                   myOrderApiDTO.foodOrderId,
                   sessionDataModel,
                   cancellationToken
                   );
                orderDataCaching.OrderCacheDataDictionary.TryRemove(myOrderApiDTO.foodOrderId, out _);
                result = new { isDeleted = "ok" };
            }
            catch (Exception ex)
            {
                result = new { isDeleted = "no" };
            }
            return Results.Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> removeOrder(IFormCollection formcollect, CancellationToken cancellationToken)
        {
            //if (utilityServices.getSessionUserId(HttpContext.Session) is null)
            //{
            //    return RedirectToAction("Login", "Index");
            //}
            var foodid = formcollect["foodId"].ToString();
            var foodorderid = formcollect["orderId"].ToString();
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            try
            {
                await orderingService.RemoveFoodOrder(
                    foodid,
                    foodorderid,
                    sessionDataModel,
                    cancellationToken
                    );
                //var foodstoremove = canteenManageContext.FoodOrders.Where(fo => fo.Id == int.Parse(formcollect["orderId"])).FirstOrDefault();
                //if (foodstoremove != null)
                //{
                //    canteenManageContext.FoodOrders.Remove(foodstoremove);
                //    await canteenManageContext.SaveChangesAsync();
                //}
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }

            return RedirectToAction("Index", new { ShowAllOrder = true });
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

            }
            return RedirectToAction("Index");
        }

    }
}
