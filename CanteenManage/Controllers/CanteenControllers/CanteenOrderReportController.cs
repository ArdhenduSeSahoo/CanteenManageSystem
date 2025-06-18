using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class CanteenOrderReportController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly OrderingService _orderingService;

        public CanteenOrderReportController(FoodListingService foodListingService, OrderingService orderingService)
        {
            this.foodListingService = foodListingService;
            _orderingService = orderingService;
        }

        /// <summary>
        /// OrderDateType is 1 for today, 2 for tomorrow, 3 for all
        /// </summary>
        /// <param name="OrderDateType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> Dashboard(string? OrderDateType, CancellationToken cancellationToken)
        {
            string panelTitle = "";
            CanteenDashboardViewDataModel model = new CanteenDashboardViewDataModel();
            var counts = foodListingService.GetOrderCounts();
            model.PanelTitle = "";
            model.TodaysCount = counts.today;
            model.TomorowCount = counts.tomorrow;
            model.AllCount = counts.all;
            if (string.IsNullOrWhiteSpace(OrderDateType))
            {
                OrderDateType = "1";
            }
            if (!string.IsNullOrWhiteSpace(OrderDateType))
            {
                if (OrderDateType == "1")
                {
                    model.BreakFastFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Breakfast, cancellationToken, false);
                    model.LunchFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Lunch, cancellationToken, false);
                    model.SnaksFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Snacks, cancellationToken, false);
                    model.PanelTitle = "Today";
                }
                else if (OrderDateType == "2")
                {
                    model.BreakFastFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Breakfast, cancellationToken, false);
                    model.LunchFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Lunch, cancellationToken, false);
                    model.SnaksFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Snacks, cancellationToken, false);
                    model.PanelTitle = "Tomorrow";
                }
                else if (OrderDateType == "3")
                {
                    model.BreakFastFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Breakfast, cancellationToken, true);
                    model.LunchFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Lunch, cancellationToken, true);
                    model.SnaksFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Snacks, cancellationToken, true);
                    model.PanelTitle = "All";
                }
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult GetOrderCounts()
        {
            var counts = foodListingService.GetOrderCounts();

            return Json(new
            {
                today = counts.today,
                tomorrow = counts.tomorrow,
                all = counts.all
            });
        }

        [HttpGet]
        public JsonResult GetOrdersByDate(string? type)
        {
            DateTime? date = null;
            if (type == "today")
                date = DateTime.Today;
            else if (type == "tomorrow")
                date = DateTime.Today.AddDays(1);
            // else keep null for "all"

            //var result = foodListingService.GetOrdersByDateAsync(date);

            //return Json(result.Select(g => new
            //{
            //    mealType = g.MealType,
            //    orders = g.Orders
            //}));
            return Json("{}");
        }


        public async Task<IActionResult> CanteenOrderReport(CancellationToken cancellationToken, int? month, int? year)
        {
            var ddl_data = await foodListingService.GetMonthListForReports(cancellationToken);
            CanteenOrderReportViewDataModel canteenOrderReportViewDataModel = new CanteenOrderReportViewDataModel();
            //canteenOrderReportViewDataModel.ReportForMonthName = ddl_data.FirstOrDefault()?.Values ?? "";
            canteenOrderReportViewDataModel.ReportMonthsDDLDataModel = ddl_data;

            //canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
            //await foodListingService.GetCanteenOrderReportData(DateTime.Now.Month, DateTime.Now.Year, cancellationToken);

            if (month != null && year != null && month != 0 && year != 0)
            {
                canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                    await foodListingService.GetCanteenOrderReportData(month.Value, year.Value, cancellationToken);
            }
            else
            {
                canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                    await foodListingService.GetCanteenOrderReportData(DateTime.Now.Month, DateTime.Now.Year, cancellationToken);
            }
            return View(canteenOrderReportViewDataModel);
        }

        public async Task<IActionResult> CanteenOrderReportDetails(DateTime orderDate, CancellationToken cancellationToken)
        {
            var data = await foodListingService.GetCanteenOrderReportDataByDateRange(orderDate, cancellationToken);
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Report(DateTime orderDate, CancellationToken cancellationToken)
        {
            var data = await foodListingService.GetCanteenOrderReportDataByDateRange(orderDate, cancellationToken);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> GetCanteenOrderReportData(IFormCollection formcollect, CancellationToken cancellationToken)
        {
            //var reportData = await foodListingService.GetCanteenOrderReportData(month, year, cancellationToken);
            int monthfromForm = 0;
            int yearfromForm = 0;

            try
            {
                var ddl_val = formcollect["DDL_Id"].ToString();
                var splitval = ddl_val.Split('_');
                monthfromForm = Convert.ToInt32(splitval[1]);
                yearfromForm = Convert.ToInt32(splitval[0]);
            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction("CanteenOrderReport", new { month = monthfromForm, year = yearfromForm });
        }

        public async Task<IActionResult> Feedback(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var feedbackList = await _orderingService.GetFeedbackList(cancellationToken);

            // Calculate pagination values
            var totalItems = feedbackList.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedItems = feedbackList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            FeedbackViewModel canteenFeedbackViewDataModel = new FeedbackViewModel
            {
                foodOrders = paginatedItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(canteenFeedbackViewDataModel);
        }


        [HttpPost]
        public async Task<IActionResult> FeedbackSubmit(int FoodOrderId, string ActionTaken, CancellationToken cancellationToken)
        {
            if (FoodOrderId > 0 && !string.IsNullOrWhiteSpace(ActionTaken))
            {
                await _orderingService.GetByIdFeedback(FoodOrderId, ActionTaken, cancellationToken);
            }

            return RedirectToAction("Feedback");
        }

        public async Task<IActionResult> FoodListMenu(string searchTerm, CancellationToken cancellationToken)
        {
            WeeklyFoodViewModel weeklyFoodViewModel = new WeeklyFoodViewModel();

            weeklyFoodViewModel.weekly1_FoodLists = await foodListingService.GetWeekWiseFoodlist(1, cancellationToken);
            weeklyFoodViewModel.weekly2_FoodLists = await foodListingService.GetWeekWiseFoodlist(2, cancellationToken);
            weeklyFoodViewModel.weekly3_FoodLists = await foodListingService.GetWeekWiseFoodlist(3, cancellationToken);
            weeklyFoodViewModel.weekly4_FoodLists = await foodListingService.GetWeekWiseFoodlist(4, cancellationToken);
            weeklyFoodViewModel.weekly5_FoodLists = await foodListingService.GetWeekWiseFoodlist(5, cancellationToken);

            return View(weeklyFoodViewModel);
        }
    }
}
