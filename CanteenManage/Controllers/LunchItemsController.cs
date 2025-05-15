using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
    public class LunchItemsController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        public LunchItemsController(CanteenManageDBContext canteenManageContext, OrderingService orderingService)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = orderingService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            int FoodID = (int)FoodTypeEnum.Lunch;
            List<DaysOfWeekModel> daysOfWeek = DateCalculationService.GetDaysOfWeek(hourBeforeDisable: 10);
            string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
            int Session_selectedDay_On_SamePage = Convert.ToInt32(HttpContext.Session.GetString(SessionConstants.UserSelectedDayOnSamePage));
            if (Session_selectedDay != null)
            {
                var selectedDate = daysOfWeek.Where(d => d.DateShort == Session_selectedDay).FirstOrDefault();
                if (selectedDate != null)
                {
                    selectedDate.IsSelected = true;
                }
                HttpContext.Session.SetString(SessionConstants.UserSelectedDayOnSamePage, "0");
            }
            else
            {
                var firstActiveDay = DateCalculationService.getFirstActiveDate(daysOfWeek);
                if (firstActiveDay != null)
                {
                    firstActiveDay.IsSelected = true;
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDay, firstActiveDay.DateShort);
                    HttpContext.Session.SetString(SessionConstants.UserSelectedDayFull, firstActiveDay.DateFull);
                }
            }
            DateTime? userSelected_DateTime_null = SessionDataHelper.getDateTimeFromSession(HttpContext.Session);
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;

            var foodOrderByUser = await orderingService.GetFoodOrdersByUserId(
                                                                SessionDataHelper.getSessionUserId(HttpContext.Session) ?? 0,
                                                                FoodID,
                                                                userSelected_DateTime,
                                                                cancellationToken
                                                                );
            var foodSnaksAll = await orderingService.GetFoodOrdersByUserId(
                                                                FoodID,
                                                                foodOrderByUser,
                                                                cancellationToken,
                                                                (int)userSelected_DateTime.DayOfWeek
                                                                );
            LunchPageDataModel lunchPageDataModel = new LunchPageDataModel();
            lunchPageDataModel.DayOfWeeks = daysOfWeek;
            lunchPageDataModel.totalCountForSelectedDay = foodOrderByUser.Sum(fo => fo.Quantity);
            lunchPageDataModel.foods = foodSnaksAll;


            return View(lunchPageDataModel);
        }
        [HttpPost]
        public IActionResult SelectDaysOfWeek(IFormCollection formcollect)
        {
            //Console.WriteLine(formcollect["selecteddate"]);
            try
            {
                HttpContext.Session.SetString(SessionConstants.UserSelectedDay, formcollect["selecteddate"].ToString());
                HttpContext.Session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
                HttpContext.Session.SetString(SessionConstants.UserSelectedDayFull, formcollect["selecteddatefull"].ToString());
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> addOrders(IFormCollection formcollect)
        {

            if (HttpContext.Session.GetString(SessionConstants.UserId) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            try
            {
                var selectedFoodId = formcollect["foodId"].ToString();
                DateTime? userSelected_DateTime_null = SessionDataHelper.getDateTimeFromSession(HttpContext.Session);
                DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
                if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
                {
                    return RedirectToAction("Index");
                }

                var userid = SessionDataHelper.getSessionUserId(HttpContext.Session);
                if (userid != null)
                {
                    //orderingService.addFoodOrder(
                    //    int.Parse(selectedFoodId),
                    //    SessionDataHelper.getSessionUserId(HttpContext.Session) ?? 0,
                    //    userSelected_DateTime
                    //    );
                }
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }
        public IActionResult removeOrders(IFormCollection formcollect)
        {
            if (HttpContext.Session.GetString(SessionConstants.UserId) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            var selectedFoodId = formcollect["foodId"].ToString();
            DateTime? userSelected_DateTime_null = SessionDataHelper.getDateTimeFromSession(HttpContext.Session);
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
            if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
            {
                return RedirectToAction("Index");
            }
            try
            {
                var existingFoodOrder = canteenManageContext.FoodOrders
                    .Where(fo => fo.FoodId == int.Parse(selectedFoodId))
                    .Where(fo => fo.EmployeeId == SessionDataHelper.getSessionUserId(HttpContext.Session))
                    .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                    .FirstOrDefault();
                if (existingFoodOrder != null)
                {
                    if (existingFoodOrder.Quantity == 1)
                    {
                        canteenManageContext.FoodOrders.Remove(existingFoodOrder);
                    }
                    else
                    {
                        existingFoodOrder.Quantity = existingFoodOrder.Quantity - 1;
                        existingFoodOrder.OrderUpdateDate = userSelected_DateTime;
                        canteenManageContext.FoodOrders.Update(existingFoodOrder);
                    }
                    canteenManageContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }
    }
}
