using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CanteenManage.Controllers
{
    public class SnacksItemListController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        //private readonly OrderingService orderingService;
        private readonly FoodListingService foodListingService;
        private readonly CartService cartService;
        private readonly UtilityServices utilityServices;
        public SnacksItemListController(FoodListingService foodListingService, CartService cartService, UtilityServices utilityServices)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            if (utilityServices.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            int snaksFoodID = (int)FoodTypeEnum.Snacks;
            List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: 15);
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));

            if (sessionDataModel.UserSelectedDay != null && Session_selectedDay_On_SamePage == 1)
            {
                var selectedDate = daysOfWeek.Where(d => d.DateShort == sessionDataModel.UserSelectedDay).FirstOrDefault();
                if (selectedDate != null)
                {
                    selectedDate.IsSelected = true;
                }

                HttpContext.Session.SetString(SessionConstants.UserSelectedDayOnSamePage, "0");
            }
            else
            {
                var firstActiveDay = daysOfWeek.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
                if (firstActiveDay != null)
                {
                    firstActiveDay.IsSelected = true;
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDay, firstActiveDay.DateShort);
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDayFull, firstActiveDay.DateFull);
                }
            }

            await cartService.CheckOutOfOrderInCart(
                                                                foodTypeEnum: FoodTypeEnum.Snacks,
                                                                sessionData: sessionDataModel,
                                                                cancellationToken: cancellationToken
                                                                );
            var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                                                                sessionDataModel.UserIdOrZero,
                                                                snaksFoodID,
                                                                sessionDataModel.UserSelectedDateOrNow,
                                                                cancellationToken
                                                                );
            var foodSnaksAll = await foodListingService.GetAllFoodList(
                                                                snaksFoodID,
                                                                foodOrderByUser,
                                                                cancellationToken,
                                                                sessionDataModel.UserSelectedDateOrNow
                                                                );


            SnaksItemPageDataModel snaksItemPageDataModel = new SnaksItemPageDataModel();
            snaksItemPageDataModel.DayOfWeeks = daysOfWeek;
            snaksItemPageDataModel.totalCountForSelectedDay = foodOrderByUser.Sum(fo => fo.Quantity);
            snaksItemPageDataModel.foods = foodSnaksAll;
            snaksItemPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                           utilityServices.getSessionUserId(HttpContext.Session) ?? 0,
                                                           cancellationToken
                                                           );
            return View(snaksItemPageDataModel);
        }
        [HttpPost]
        public IActionResult SelectDaysOfWeek(IFormCollection formcollect)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(HttpContext.Session,
                                  formcollect["selecteddate"].ToString(),
                                  formcollect["selecteddatefull"].ToString());
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }
    }
}