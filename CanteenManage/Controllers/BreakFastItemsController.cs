namespace CanteenManage.Controllers
{
    using System;
    using System.Threading.Tasks;
    using CanteenManage.CanteenRepository.Contexts;
    using CanteenManage.Models;
    using CanteenManage.Services;
    using CanteenManage.Utility;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [Authorize(Roles = "Employee")]
    public class BreakFastItemsController : Controller
    {

        private readonly FoodListingService foodListingService;

        private readonly CartService cartService;

        private readonly UtilityServices utilityServices;
        private readonly ILogger<BreakFastItemsController> logger;

        public BreakFastItemsController(FoodListingService foodListingService, CartService cartService, UtilityServices utilityServices, ILogger<BreakFastItemsController> logger)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
            this.logger = logger;
        }

        /// <summary>
        /// The Index
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        public async Task<IActionResult> Index(CancellationToken cancellationToken, int Dsosp = 0)
        {
            BreakFastPageDataModel breakFastPageDataModel = new BreakFastPageDataModel();
            breakFastPageDataModel.DayOfWeeks = new List<DaysOfWeekModel>();
            breakFastPageDataModel.totalCountForSelectedDay = 0;
            breakFastPageDataModel.foods = new List<FoodDetails>();
            breakFastPageDataModel.CartItemCount = 0;
            try
            {
                int FoodType = (int)FoodTypeEnum.Breakfast;
                List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: CustomDataConstants.BreakfastTimeHour);
                //string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);

                if (!string.IsNullOrEmpty(sessionDataModel.UserSelectedDay) && Dsosp == 1)
                {
                    var selectedDate = daysOfWeek.Where(d => d.DateShort == sessionDataModel.UserSelectedDay).FirstOrDefault();
                    if (selectedDate != null)
                    {
                        selectedDate.IsSelected = true;
                    }
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
                                                                    sessionData: sessionDataModel,
                                                                    cancellationToken: cancellationToken
                                                                    );
                //var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                //                                                    sessionDataModel.UserIdOrZero,
                //                                                    FoodType,
                //                                                    sessionDataModel.UserSelectedDateOrNow,
                //                                                    cancellationToken
                //                                                    );
                var foodDetailsAll = await foodListingService.GetAllFoodList(
                                                                    FoodType,
                                                                    cancellationToken,
                                                                    sessionDataModel.UserSelectedDateOrNow,
                                                                    sessionData: sessionDataModel
                                                                    );

                breakFastPageDataModel.DayOfWeeks = daysOfWeek;
                breakFastPageDataModel.totalCountForSelectedDay = foodDetailsAll.Sum(fo => fo.FoodCountInCart);
                breakFastPageDataModel.foods = foodDetailsAll;
                breakFastPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                                    sessionDataModel.UserId ?? 0,
                                                                    cancellationToken
                                                                    );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in BreakFastItemsController Index method: {Message}", ex.Message);
            }
            return View(breakFastPageDataModel);
        }

        /// <summary>
        /// The SelectDaysOfWeek
        /// </summary>
        /// <param name="formcollect">The formcollect<see cref="IFormCollection"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpPost]
        public IActionResult SelectDaysOfWeek(string selecteddate, string selecteddatefull)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.BreakfastTimeHour,
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
