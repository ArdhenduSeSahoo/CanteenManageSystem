using CanteenManage.Models;
using CanteenManage.Repo.Contexts;
using CanteenManage.Repo.Models;
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
        public async Task<IActionResult> Index()
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            int FoodID = 2;
            List<DaysOfWeekModel> daysOfWeek = DateCalculationService.GetDaysOfWeek(hourBeforeDisable: 10);
            string? Session_selectedDay = HttpContext.Session.GetString(SessionConstants.UserSelectedDay);
            if (Session_selectedDay != null)
            {
                var selectedDate = daysOfWeek.Where(d => d.DateShort == Session_selectedDay).FirstOrDefault();
                if (selectedDate != null)
                {
                    selectedDate.IsSelected = true;
                }
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
                                                                userSelected_DateTime
                                                                );
            var foodSnaksAll = await orderingService.GetFoodOrdersByUserId(
                                                                FoodID,
                                                                foodOrderByUser
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
                    .Include(fo => fo.Food)
                    .Where(fo => fo.FoodId == int.Parse(selectedFoodId))
                    .Where(fo => fo.EmployeId == SessionDataHelper.getSessionUserId(HttpContext.Session))
                    .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                    .FirstOrDefault();


                //if (existingFoodOrder != null && existingFoodOrder?.Quantity < 5)
                //{
                if (existingFoodOrder != null && existingFoodOrder?.Quantity < 5)
                {
                    existingFoodOrder.Quantity = existingFoodOrder.Quantity + 1;
                    existingFoodOrder.TotalPrice = existingFoodOrder.Quantity * existingFoodOrder.Food.Price;
                    existingFoodOrder.OrderUpdateDate = DateTime.Now;
                    canteenManageContext.FoodOrders.Update(existingFoodOrder);
                }
                else
                {
                    //fixing time to 6:00 AM for filtering in orderlist screen
                    TimeSpan ts = new TimeSpan(11, 05, 0);
                    FoodOrder foodOrder = new FoodOrder();
                    foodOrder.FoodId = int.Parse(selectedFoodId);
                    foodOrder.EmployeId = SessionDataHelper.getSessionUserId(HttpContext.Session) ?? 0;
                    foodOrder.OrderDate = userSelected_DateTime.Date + ts;
                    foodOrder.OrderUpdateDate = DateTime.Now;
                    foodOrder.Quantity = 1;

                    var foodprice = await canteenManageContext.Foods
                            .Where(f => f.Id == int.Parse(selectedFoodId))
                            .Select(f => f.Price)
                            .FirstOrDefaultAsync();
                    foodOrder.TotalPrice = foodOrder.Quantity * foodprice;
                    canteenManageContext.FoodOrders.Add(foodOrder);
                }
                canteenManageContext.SaveChanges();

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
                    .Where(fo => fo.EmployeId == SessionDataHelper.getSessionUserId(HttpContext.Session))
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
