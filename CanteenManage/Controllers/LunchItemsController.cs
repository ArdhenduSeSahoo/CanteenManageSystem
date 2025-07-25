using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class LunchItemsController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        //private readonly OrderingService orderingService;
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        private readonly CartService cartService;
        private readonly ILogger<LunchItemsController> logger;
        public LunchItemsController(FoodListingService foodListingService, UtilityServices utilityServices, CartService cartService, ILogger<LunchItemsController> logger)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
            this.cartService = cartService;
            this.logger = logger;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken, int Dsosp = 0)
        {
            LunchPageDataModel lunchPageDataModel = new LunchPageDataModel();
            lunchPageDataModel.DayOfWeeks = new List<DaysOfWeekModel>();
            lunchPageDataModel.totalCountForSelectedDay = 0;
            lunchPageDataModel.foods = new List<FoodDetails>();
            lunchPageDataModel.CartItemCount = 0;

            try
            {
                int FoodID = (int)FoodTypeEnum.Lunch;
                List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: CustomDataConstants.LunchTimeHour);
                //string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                //int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));
                if (!string.IsNullOrEmpty(sessionDataModel.UserSelectedDay) && Dsosp == 1)
                {
                    var selectedDate = daysOfWeek.Where(d => d.DateShort == sessionDataModel.UserSelectedDay).FirstOrDefault();
                    if (selectedDate != null)
                    {
                        selectedDate.IsSelected = true;
                        lunchPageDataModel.showAddBtn = selectedDate.IsActiveDay;
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
                        lunchPageDataModel.showAddBtn = firstActiveDay.IsActiveDay;
                    }
                    sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                }

                await cartService.CheckOutOfOrderInCart(
                                                                    sessionData: sessionDataModel,
                                                                    cancellationToken: cancellationToken
                                                                    );
                //var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                //                                                    sessionDataModel.UserIdOrZero,
                //                                                    FoodID,
                //                                                    sessionDataModel.UserSelectedDateOrNow,
                //                                                    cancellationToken
                //                                                    );
                var foodDetailsAll = await foodListingService.GetAllFoodList(
                                                                    FoodTypeEnum.Lunch,
                                                                    cancellationToken,
                                                                    sessionDataModel.UserSelectedDateOrNow,
                                                                    sessionData: sessionDataModel
                                                                    );

                lunchPageDataModel.DayOfWeeks = daysOfWeek;
                lunchPageDataModel.totalCountForSelectedDay = foodDetailsAll.Sum(fo => fo.FoodCountInCart);
                lunchPageDataModel.foods = foodDetailsAll;
                lunchPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                               utilityServices.getSessionUserId(HttpContext.Session) ?? 0,
                                                               cancellationToken
                                                               );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in LunchItemsController Index method" + ex.Message);
            }
            return View(lunchPageDataModel);
        }



        [HttpPost]
        public IActionResult SelectDaysOfWeek(string selecteddate, string selecteddatefull)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.LunchTimeHour,
                    HttpContext.Session,
                    selectedDay: selecteddate,
                    selectedDate: selecteddatefull
                    );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in BreakFastItemsController SelectDaysOfWeek method: {Message}", ex.Message);
            }

            return RedirectToAction("Index", new { Dsosp = 1 });
        }

    }
}
