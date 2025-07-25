using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class DinnerController : Controller
    {
        private readonly FoodListingService foodListingService;

        private readonly CartService cartService;

        private readonly UtilityServices utilityServices;
        private readonly ILogger<DinnerController> logger;

        public DinnerController(FoodListingService foodListingService, CartService cartService, UtilityServices utilityServices, ILogger<DinnerController> logger)
        {
            this.foodListingService = foodListingService;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken, int Dsosp = 0)
        {
            DinnerPageDataModel dinnerPageDataModel = new DinnerPageDataModel();
            dinnerPageDataModel.DayOfWeeks = new List<DaysOfWeekModel>();
            dinnerPageDataModel.totalCountForSelectedDay = 0;
            dinnerPageDataModel.foods = new List<FoodDetails>();
            //breakFastPageDataModel.CartItemCount = 0;

            try
            {
                List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: CustomDataConstants.DinnerTimeHour);
                //string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);

                if (!string.IsNullOrEmpty(sessionDataModel.UserSelectedDay) && Dsosp == 1)
                {
                    var selectedDate = daysOfWeek.Where(d => d.DateShort == sessionDataModel.UserSelectedDay).FirstOrDefault();
                    if (selectedDate != null)
                    {
                        selectedDate.IsSelected = true;
                        dinnerPageDataModel.showAddBtn = selectedDate.IsActiveDay;
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
                        dinnerPageDataModel.showAddBtn = firstActiveDay.IsActiveDay;
                    }
                    sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                }
                await cartService.CheckOutOfOrderInCart(
                                                                    sessionData: sessionDataModel,
                                                                    cancellationToken: cancellationToken
                                                                    );

                var foodDetailsAll = await foodListingService.GetAllFoodList(
                                                                    FoodTypeEnum.Dinner,
                                                                    cancellationToken,
                                                                    sessionDataModel.UserSelectedDateOrNow,
                                                                    sessionData: sessionDataModel
                                                                    );

                dinnerPageDataModel.DayOfWeeks = daysOfWeek;
                dinnerPageDataModel.totalCountForSelectedDay = foodDetailsAll.Sum(fo => fo.FoodCountInCart);
                dinnerPageDataModel.foods = foodDetailsAll;
                dinnerPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                                    sessionDataModel.UserId ?? 0,
                                                                    cancellationToken
                                                                    );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Dinner index page.");
            }
            return View(dinnerPageDataModel);
        }
        [HttpPost]
        public IActionResult SelectDaysOfWeek(string selecteddate, string selecteddatefull)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                utilityServices.SetDateTimeToSession(
                    CustomDataConstants.DinnerTimeHour,
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
