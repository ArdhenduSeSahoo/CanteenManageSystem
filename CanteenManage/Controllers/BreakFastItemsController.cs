namespace CanteenManage.Controllers
{
    using System.Threading.Tasks;
    using CanteenManage.CanteenRepository.Contexts;
    using CanteenManage.Models;
    using CanteenManage.Services;
    using CanteenManage.Utility;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="BreakFastItemsController" />
    /// </summary>
    public class BreakFastItemsController : Controller
    {

        private readonly FoodListingService foodListingService;

        /// <summary>
        /// Defines the cartService
        /// </summary>
        private readonly CartService cartService;

        /// <summary>
        /// Defines the utilityServices
        /// </summary>
        private readonly UtilityServices utilityServices;

        public BreakFastItemsController(FoodListingService foodListingService, CartService cartService, UtilityServices utilityServices)
        {
            //this.canteenManageContext = canteenManageContext;
            //this.orderingService = orderingService;
            this.foodListingService = foodListingService;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
        }

        /// <summary>
        /// The Index
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{IActionResult}"/></returns>
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            int FoodType = (int)FoodTypeEnum.Breakfast;
            List<DaysOfWeekModel> daysOfWeek = utilityServices.GetDaysOfWeek(hourBeforeDisable: 6);
            //string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));
            if (sessionDataModel.UserSelectedDay != null)
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
                                                                foodTypeEnum: FoodTypeEnum.Breakfast,
                                                                sessionData: sessionDataModel,
                                                                cancellationToken: cancellationToken
                                                                );
            var foodOrderByUser = await foodListingService.GetCartFoodOrdersByUser(
                                                                sessionDataModel.UserIdOrZero,
                                                                FoodType,
                                                                sessionDataModel.UserSelectedDateOrNow,
                                                                cancellationToken
                                                                );
            var foodSnaksAll = await foodListingService.GetAllFoodList(
                                                                FoodType,
                                                                foodOrderByUser,
                                                                cancellationToken,
                                                                sessionDataModel.UserSelectedDateOrNow
                                                                );
            BreakFastPageDataModel breakFastPageDataModel = new BreakFastPageDataModel();
            breakFastPageDataModel.DayOfWeeks = daysOfWeek;
            breakFastPageDataModel.totalCountForSelectedDay = foodOrderByUser.Sum(fo => fo.Quantity);
            breakFastPageDataModel.foods = foodSnaksAll;
            breakFastPageDataModel.CartItemCount = await foodListingService.GetCartItemCount(
                                                                sessionDataModel.UserId ?? 0,
                                                                cancellationToken
                                                                );
            return View(breakFastPageDataModel);
        }

        /// <summary>
        /// The SelectDaysOfWeek
        /// </summary>
        /// <param name="formcollect">The formcollect<see cref="IFormCollection"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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
