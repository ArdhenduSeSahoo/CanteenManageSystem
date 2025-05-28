using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManage.Controllers
{
    //[Authorize(Roles = "Employee")]
    public class LunchItemsController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        //private readonly OrderingService orderingService;
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        private readonly CartService cartService;
        public LunchItemsController(FoodListingService foodListingService, UtilityServices utilityServices, CartService cartService)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
            this.cartService = cartService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken, int Dsosp = 0)
        {

            int FoodID = (int)FoodTypeEnum.Lunch;
            List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: CustomDataConstants.LunchTimeHour);
            //string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));
            if (sessionDataModel.UserSelectedDay != null && Dsosp == 1)
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
                var firstActiveDay = utilityServices.getFirstActiveDate(daysOfWeek);
                if (firstActiveDay != null)
                {
                    firstActiveDay.IsSelected = true;
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDay, firstActiveDay.DateShort);
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDayFull, firstActiveDay.DateFull);
                }
                sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            }

            await cartService.CheckOutOfOrderInCart(
                                                                foodTypeEnum: FoodTypeEnum.Lunch,
                                                                sessionData: sessionDataModel,
                                                                cancellationToken: cancellationToken
                                                                );
            var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                                                                sessionDataModel.UserIdOrZero,
                                                                FoodID,
                                                                sessionDataModel.UserSelectedDateOrNow,
                                                                cancellationToken
                                                                );
            var foodSnaksAll = await foodListingService.GetAllFoodList(
                                                                FoodID,
                                                                foodOrderByUser,
                                                                cancellationToken,
                                                                sessionDataModel.UserSelectedDateOrNow
                                                                );
            LunchPageDataModel lunchPageDataModel = new LunchPageDataModel();
            lunchPageDataModel.DayOfWeeks = daysOfWeek;
            lunchPageDataModel.totalCountForSelectedDay = foodOrderByUser.Sum(fo => fo.Quantity);
            lunchPageDataModel.foods = foodSnaksAll;
            lunchPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                           utilityServices.getSessionUserId(HttpContext.Session) ?? 0,
                                                           cancellationToken
                                                           );

            return View(lunchPageDataModel);
        }

        public IActionResult SelectDaysOfWeek(string selectedDate, string selectedFullDate)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.LunchTimeHour,
                    HttpContext.Session,
                    selectedDate,
                    selectedFullDate
                    );
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index", new { Dsosp = 1 });
        }

        [HttpPost]
        public IActionResult SelectDaysOfWeek(IFormCollection formcollect)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.LunchTimeHour,
                    HttpContext.Session,
                   formcollect["selecteddate"].ToString(),
                   formcollect["selecteddatefull"].ToString());
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index", new { Dsosp = 1 });
        }

    }
}
