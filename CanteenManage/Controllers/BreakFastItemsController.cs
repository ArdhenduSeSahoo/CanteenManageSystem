using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
    public class BreakFastItemsController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        public BreakFastItemsController(CanteenManageDBContext canteenManageContext, OrderingService orderingService)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = orderingService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            int FoodID = 1;
            List<DaysOfWeekModel> daysOfWeek = DateCalculationService.GetDaysOfWeek(hourBeforeDisable: 6);
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
                var firstActiveDay = daysOfWeek.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
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
            BreakFastPageDataModel breakFastPageDataModel = new BreakFastPageDataModel();
            breakFastPageDataModel.DayOfWeeks = daysOfWeek;
            breakFastPageDataModel.totalCountForSelectedDay = foodOrderByUser.Sum(fo => fo.Quantity);
            breakFastPageDataModel.foods = foodSnaksAll;
            return View(breakFastPageDataModel);
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

    }
}
