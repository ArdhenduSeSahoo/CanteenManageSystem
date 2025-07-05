using System.Threading.Tasks;
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
    public class SnacksItemListController : Controller
    {
        //private readonly CanteenManageDBContext canteenManageContext;
        //private readonly OrderingService orderingService;
        private readonly FoodListingService foodListingService;
        private readonly CartService cartService;
        private readonly UtilityServices utilityServices;
        private readonly ILogger<SnacksItemListController> logger;
        public SnacksItemListController(FoodListingService foodListingService, CartService cartService, UtilityServices utilityServices, ILogger<SnacksItemListController> logger)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
            this.logger = logger;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken, int Dsosp = 0)
        {
            SnaksItemPageDataModel snaksItemPageDataModel = new SnaksItemPageDataModel();
            snaksItemPageDataModel.DayOfWeeks = new List<DaysOfWeekModel>();
            snaksItemPageDataModel.totalCountForSelectedDay = 0;
            snaksItemPageDataModel.foods = new List<FoodDetails>();
            snaksItemPageDataModel.CartItemCount = 0;
            try
            {



                //if (utilityServices.getSessionUserId(HttpContext.Session) is null)
                //{
                //    return RedirectToAction("Login", "Index");
                //}
                int snaksFoodID = (int)FoodTypeEnum.Snacks;
                List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: CustomDataConstants.SnacksTimeHour);
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));

                if (!string.IsNullOrEmpty(sessionDataModel.UserSelectedDay) && Dsosp == 1)
                {
                    var selectedDate = daysOfWeek.Where(d => d.DateShort == sessionDataModel.UserSelectedDay).FirstOrDefault();
                    //utilityServices.getFirstActiveDate(daysOfWeek);
                    if (selectedDate != null)
                    {
                        selectedDate.IsSelected = true;
                    }

                    HttpContext.Session.SetString(SessionConstants.UserSelectedDayOnSamePage, "0");
                }
                else
                {
                    var firstActiveDay = utilityServices.getFirstActiveDate(daysOfWeek);
                    // daysOfWeek.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
                    if (firstActiveDay != null)
                    {
                        firstActiveDay.IsSelected = true;
                        HttpContext.Session.SetString(SessionConstants.UserSelectedDay, firstActiveDay.DateShort);
                        HttpContext.Session.SetString(SessionConstants.UserSelectedDayFull, firstActiveDay.DateFull);
                    }
                    sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                }

                await cartService.CheckOutOfOrderInCart(
                                                                    sessionData: sessionDataModel,
                                                                    cancellationToken: cancellationToken
                                                                    );
                //var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                //                                                    sessionDataModel.UserIdOrZero,
                //                                                    snaksFoodID,
                //                                                    sessionDataModel.UserSelectedDateOrNow,
                //                                                    cancellationToken
                //                                                    );
                var foodDetailsAll = await foodListingService.GetAllFoodList(
                                                                    snaksFoodID,
                                                                    cancellationToken,
                                                                    sessionDataModel.UserSelectedDateOrNow,
                                                                    sessionData: sessionDataModel
                                                                    );

                snaksItemPageDataModel.DayOfWeeks = daysOfWeek;
                snaksItemPageDataModel.totalCountForSelectedDay = foodDetailsAll.Sum(fo => fo.FoodCountInCart);
                snaksItemPageDataModel.foods = foodDetailsAll;
                snaksItemPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                               utilityServices.getSessionUserId(HttpContext.Session) ?? 0,
                                                               cancellationToken
                                                               );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SnacksItemListController Index method" + ex.Message + "---" + ex.StackTrace);
            }
            return View(snaksItemPageDataModel);
        }


        [HttpPost]
        public IActionResult SelectDaysOfWeek(string selecteddate, string selecteddatefull)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.SnacksTimeHour,
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