using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Repo.Contexts;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
    public class MyOrdersController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        public MyOrdersController(CanteenManageDBContext canteenManageContext)
        {
            this.canteenManageContext = canteenManageContext;
        }
        public async Task<IActionResult> Index()
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            MyOrderViewDataModel myOrderViewDataModel = new MyOrderViewDataModel();
            try
            {
                TimeSpan ts = new TimeSpan(06, 00, 0);
                DateTime snacks_dateTime = DateTime.Now.Date + ts;
                List<DaysOfWeekModel> daysOfWeek = DateCalculationService.GetDaysOfWeek();
                var daysOfWeek_for_snaks = daysOfWeek.Where(
                    d =>
                    d.DateTime >= snacks_dateTime
                    ).ToList();

                var snacksorders = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .Where(
                    fo => fo.Food.FoodTypeId == 3
                    && fo.EmployeId == SessionDataHelper.getSessionUserId(HttpContext.Session)
                    && daysOfWeek_for_snaks.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .ToListAsync();
                myOrderViewDataModel.SnaksFoodOrders = snacksorders;
                ////////////////////////////////////////////////////////////
                ts = new TimeSpan(11, 00, 0);
                DateTime lunch_dateTime = DateTime.Now.Date + ts;

                var daysOfWeek_for_lunch = daysOfWeek.Where(
                    d =>
                    d.DateTime >= snacks_dateTime
                    ).ToList();

                var lunchorders = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .Where(
                    fo => fo.Food.FoodTypeId == 2
                    && fo.EmployeId == SessionDataHelper.getSessionUserId(HttpContext.Session)
                    && daysOfWeek_for_lunch.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .ToListAsync();
                myOrderViewDataModel.LunchFoodOrders = lunchorders;
                ////////////////////////////////////////////////////////////
                ts = new TimeSpan(15, 00, 0);
                DateTime breakfast_dateTime = DateTime.Now.Date + ts;

                var daysOfWeek_for_breakfast = daysOfWeek.Where(
                    d => d.DateTime >= snacks_dateTime
                    ).ToList();

                var breakfastorders = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .Where(
                    fo => fo.Food.FoodTypeId == 1
                    && fo.EmployeId == SessionDataHelper.getSessionUserId(HttpContext.Session)
                    && daysOfWeek_for_breakfast.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .ToListAsync();
                myOrderViewDataModel.BreakFastFoodOrders = breakfastorders;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(myOrderViewDataModel);
        }

        [HttpPost]
        public async Task<IActionResult> removeOrder(IFormCollection formcollect)
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            try
            {
                var foodstoremove = canteenManageContext.FoodOrders.Where(fo => fo.Id == int.Parse(formcollect["orderId"])).FirstOrDefault();
                if (foodstoremove != null)
                {
                    canteenManageContext.FoodOrders.Remove(foodstoremove);
                    await canteenManageContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }
    }
}
