using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authorization;
using CanteenManage.CanteenRepository.Models;

namespace CanteenManage.Controllers
{
    //[Authorize(Roles = "Employee")]
    public class MyOrdersController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly UtilityServices utilityServices;
        private readonly FoodListingService foodListingService;
        public MyOrdersController(CanteenManageDBContext canteenManageContext, UtilityServices utility, FoodListingService foodListingService)
        {
            this.canteenManageContext = canteenManageContext;
            this.utilityServices = utility;
            this.foodListingService = foodListingService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken, bool ShowAllOrder=false)
        {

            MyOrderViewDataModel myOrderViewDataModel = new MyOrderViewDataModel();
            myOrderViewDataModel.ShowAllOrder = ShowAllOrder;
            try
            {

                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                DateTime snacks_dateTime = DateTime.Now.Date;

                List<FoodOrder> snacksorders = new List<FoodOrder>();
                List<FoodOrder> lunchorders = new List<FoodOrder>();
                List<FoodOrder> breakfastorders = new List<FoodOrder>();

                if (ShowAllOrder)
                {
                    snacksorders = await foodListingService.GetEmployFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Snacks,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    snacksorders = await foodListingService.GetEmployFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Snacks,
                                                                    cancellationToken
                                                                    );
                }
                myOrderViewDataModel.SnaksFoodOrders = snacksorders;
                ////////////////////////////////////////////////////////////

                if (ShowAllOrder)
                {
                    lunchorders = await foodListingService.GetEmployFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Lunch,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    lunchorders = await foodListingService.GetEmployFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Lunch,
                                                                    cancellationToken
                                                                    );
                }
                myOrderViewDataModel.LunchFoodOrders = lunchorders;
                ////////////////////////////////////////////////////////////
                if (ShowAllOrder)
                {
                    breakfastorders = await foodListingService.GetEmployFoodOrdersAll(sessionDataModel.UserIdOrZero,
                                                                FoodTypeEnum.Breakfast,
                                                                cancellationToken
                                                                );
                }
                else
                {

                    breakfastorders = await foodListingService.GetEmployFoodOrdersToday(sessionDataModel.UserIdOrZero,
                                                                    FoodTypeEnum.Breakfast,
                                                                    cancellationToken
                                                                    );
                }
                myOrderViewDataModel.BreakFastFoodOrders = breakfastorders;

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
        public async Task<IActionResult> removeOrder(int orderId, bool showAllOrder)
        {
            try
            {
                var foodstoremove = canteenManageContext.FoodOrders
                                    .FirstOrDefault(fo => fo.Id == orderId);
                if (foodstoremove != null)
                {
                    canteenManageContext.FoodOrders.Remove(foodstoremove);
                    await canteenManageContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
            }

            return RedirectToAction("Index", new { ShowAllOrder = showAllOrder });
        }

    }
}
