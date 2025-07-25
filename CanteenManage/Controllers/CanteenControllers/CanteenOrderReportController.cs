using System.Globalization;
using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            var counts = await foodListingService.GetOrderCounts(cancellationToken);
            model.PanelTitle = "";
            model.TodaysTotalCount = counts.TodayTotal;
            model.TodaysCompletedCount = counts.TodayTotalCompleted;
            model.TodaysUnCompletedCount = counts.TodayTotalUnCompleted;
            model.TomorrowCount = counts.tomorrow;
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
                    model.DinnerFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Dinner, cancellationToken, false);
                    model.PanelTitle = "Today";
                }
                else if (OrderDateType == "2")
                {
                    model.BreakFastFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Breakfast, cancellationToken, false);
                    model.LunchFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Lunch, cancellationToken, false);
                    model.SnaksFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Snacks, cancellationToken, false);
                    model.DinnerFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now.AddDays(1), foodTypeEnum: FoodTypeEnum.Dinner, cancellationToken, false);
                    model.PanelTitle = "Tomorrow";
                }
                else if (OrderDateType == "3")
                {
                    model.BreakFastFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Breakfast, cancellationToken, true);
                    model.LunchFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Lunch, cancellationToken, true);
                    model.SnaksFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Snacks, cancellationToken, true);
                    model.DinnerFoodOrders = await foodListingService.GetOrdersByDateAsync(DateTime.Now, foodTypeEnum: FoodTypeEnum.Dinner, cancellationToken, true);
                    model.PanelTitle = "All";
                }
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult GetOrderCounts()
        {
            //var counts = foodListingService.GetOrderCounts();

            //return Json(new
            //{
            //    today = counts.today,
            //    tomorrow = counts.tomorrow,
            //    all = counts.all
            //});
            return Json(new { today = 0, tomorrow = 0, all = 0 }); // Placeholder for actual implementation
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


        public async Task<IActionResult> CanteenOrderReport(CancellationToken cancellationToken, string? fromDatePicker, string? toDatePicker, string? orderStatusOptions)
        {
            //var ddl_data = await foodListingService.GetMonthListForReports(cancellationToken);
            CanteenOrderReportViewDataModel canteenOrderReportViewDataModel = new CanteenOrderReportViewDataModel();
            canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels = new List<CanteenOrdersReportTableViewDataModel>();

            // Convert a null string.
            string dateString = null;
            CultureInfo provider = CultureInfo.InvariantCulture;


            try
            {
                if (string.IsNullOrEmpty(orderStatusOptions))
                {
                    orderStatusOptions = "1"; // Default to "1" if no option is selected
                }

                if (!string.IsNullOrEmpty(fromDatePicker) && !string.IsNullOrEmpty(toDatePicker))
                {
                    dateString = fromDatePicker;
                    DateTime dateTimeFrom = DateTime.ParseExact(fromDatePicker, "MM-dd-yyyy", provider);
                    DateTime dateTimeTo = DateTime.ParseExact(toDatePicker, "MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                    await foodListingService.GetOrderReport(dateTimeFrom, dateTimeTo, orderStatusOptions, cancellationToken);
                    canteenOrderReportViewDataModel.fromDatePicker = dateTimeFrom.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.toDatePicker = dateTimeTo.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.orderStatusOptions = orderStatusOptions ?? "1";
                }
                else
                {
                    canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                        await foodListingService.GetOrderReport(DateTime.Now.AddDays(-30), DateTime.Now, orderStatusOptions, cancellationToken);
                    canteenOrderReportViewDataModel.fromDatePicker = DateTime.Now.AddDays(-30).ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.toDatePicker = DateTime.Now.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.orderStatusOptions = "1";
                }

            }
            catch (Exception ex)
            {

            }

            return View(canteenOrderReportViewDataModel);
        }
        public async Task<IActionResult> OrderReportNonSub(CancellationToken cancellationToken, string? fromDatePicker, string? toDatePicker, string? orderStatusOptions)
        {
            //var ddl_data = await foodListingService.GetMonthListForReports(cancellationToken);
            CanteenOrderReportViewDataModel canteenOrderReportViewDataModel = new CanteenOrderReportViewDataModel();
            canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels = new List<CanteenOrdersReportTableViewDataModel>();

            // Convert a null string.
            string dateString = null;
            CultureInfo provider = CultureInfo.InvariantCulture;


            try
            {
                if (string.IsNullOrEmpty(orderStatusOptions))
                {
                    orderStatusOptions = "1"; // Default to "1" if no option is selected
                }

                if (!string.IsNullOrEmpty(fromDatePicker) && !string.IsNullOrEmpty(toDatePicker))
                {
                    dateString = fromDatePicker;
                    DateTime dateTimeFrom = DateTime.ParseExact(fromDatePicker, "MM-dd-yyyy", provider);
                    DateTime dateTimeTo = DateTime.ParseExact(toDatePicker, "MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                    await foodListingService.GetOrderReport(dateTimeFrom, dateTimeTo,
                                                                        orderStatusOptions,
                                                                        cancellationToken,
                                                                        true);
                    canteenOrderReportViewDataModel.fromDatePicker = dateTimeFrom.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.toDatePicker = dateTimeTo.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.orderStatusOptions = orderStatusOptions ?? "1";
                }
                else
                {
                    canteenOrderReportViewDataModel.canteenOrdersReportTableViewDataModels =
                        await foodListingService.GetOrderReport(DateTime.Now.AddDays(-30), DateTime.Now, orderStatusOptions, cancellationToken, true);
                    canteenOrderReportViewDataModel.fromDatePicker = DateTime.Now.AddDays(-30).ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.toDatePicker = DateTime.Now.ToString("MM-dd-yyyy", provider);
                    canteenOrderReportViewDataModel.orderStatusOptions = "1";
                }

            }
            catch (Exception ex)
            {

            }

            return View(canteenOrderReportViewDataModel);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken, string? fromDatePicker, string? toDatePicker, string? orderStatusOptions, bool IsNonSubsidiary = false)
        {
            List<CanteenOrdersReportTableViewDataModel> canteenOrderReportViewDataModels = new List<CanteenOrdersReportTableViewDataModel>();
            if (!string.IsNullOrWhiteSpace(fromDatePicker) && !string.IsNullOrWhiteSpace(toDatePicker) && !string.IsNullOrWhiteSpace(orderStatusOptions))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dateTimeFrom = DateTime.ParseExact(fromDatePicker, "MM-dd-yyyy", provider);
                DateTime dateTimeTo = DateTime.ParseExact(toDatePicker, "MM-dd-yyyy", provider);
                canteenOrderReportViewDataModels = await foodListingService.GetOrderReport(dateTimeFrom, dateTimeTo, orderStatusOptions, cancellationToken, OnlyNonSubsidiary: IsNonSubsidiary);
            }
            if (canteenOrderReportViewDataModels.Count > 0)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("OrderList");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "OrderDate";
                    worksheet.Cell(currentRow, 2).Value = "Order Count";
                    worksheet.Cell(currentRow, 3).Value = "Employee Count";
                    worksheet.Cell(currentRow, 4).Value = "TotalPrice";
                    worksheet.Cell(currentRow, 5).Value = "TotalEmployeePrice";
                    worksheet.Cell(currentRow, 6).Value = "TotalSubsidyPrice";


                    for (int i = 0; i < canteenOrderReportViewDataModels.Count; i++)
                    {
                        {
                            currentRow++;
                            if (currentRow == (canteenOrderReportViewDataModels.Count + 1))
                            {
                                worksheet.Cell(currentRow, 1).Value = "Total";
                            }
                            else
                            {
                                worksheet.Cell(currentRow, 1).Value = canteenOrderReportViewDataModels[i].OrderDate.ToString("dd-MM-yyyy");
                            }

                            worksheet.Cell(currentRow, 2).Value = canteenOrderReportViewDataModels[i].TotalOrderCount;
                            worksheet.Cell(currentRow, 3).Value = canteenOrderReportViewDataModels[i].TotalEmployeeCount;
                            worksheet.Cell(currentRow, 4).Value = canteenOrderReportViewDataModels[i].TotalPrice;
                            worksheet.Cell(currentRow, 5).Value = canteenOrderReportViewDataModels[i].TotalEmployeePrice;
                            worksheet.Cell(currentRow, 6).Value = canteenOrderReportViewDataModels[i].TotalSubsidyPrice;

                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        var subsidiaryName = IsNonSubsidiary ? "NonSub_" : "";
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "OrderReport_" + subsidiaryName + fromDatePicker + "_" + toDatePicker + "_" + ".xlsx");
                    }
                }
            }
            return this.RedirectToAction("EmpOrderReport", new { fromDatePicker = fromDatePicker, toDatePicker = toDatePicker });

        }

        public async Task<IActionResult> OrderReportDetails(DateTime orderDate, string orderStatusOptions, CancellationToken cancellationToken, bool IsNonSubsidiary = false)
        {
            FoodReportViewModel foodReportViewModel = new FoodReportViewModel
            {
                ReportDate = orderDate.ToString("dd-MM-yyyy"),
                FoodOrdersDetails = new List<FoodReportDetailsViewModel>()
            };
            foodReportViewModel.FoodOrdersDetails = await foodListingService.GetOrderReportByDate(orderDate, orderStatusOptions, cancellationToken, IncludeSubsidiary: IsNonSubsidiary);

            return View(foodReportViewModel);
        }
        public async Task<IResult> OrderReportDetailsApi(DateTime orderDate, string orderStatusOptions, CancellationToken cancellationToken, bool IsNonSubsidiary = false)
        {
            FoodReportViewModel foodReportViewModel = new FoodReportViewModel
            {
                ReportDate = orderDate.ToString("dd-MM-yyyy"),
                FoodOrdersDetails = new List<FoodReportDetailsViewModel>()
            };
            foodReportViewModel.FoodOrdersDetails = await foodListingService.GetOrderReportByDate(orderDate, orderStatusOptions, cancellationToken, IncludeSubsidiary: IsNonSubsidiary);

            return Results.Ok(JsonConvert.SerializeObject(foodReportViewModel));
        }
        public async Task<IActionResult> CanteenOrderReportDetails(DateTime orderDate, CancellationToken cancellationToken)
        {
            FoodReportViewModel foodReportViewModel = new FoodReportViewModel
            {
                ReportDate = orderDate.ToString("dd-MM-yyyy"),
                FoodOrdersDetails = new List<FoodReportDetailsViewModel>()
            };
            foodReportViewModel.FoodOrdersDetails = await foodListingService.GetOrderReportByDate(orderDate, "1", cancellationToken);

            return View(foodReportViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Report(DateTime orderDate, CancellationToken cancellationToken)
        {
            FoodReportViewModel foodReportViewModel = new FoodReportViewModel
            {
                ReportDate = orderDate.ToString("dd-MM-yyyy"),
                FoodOrdersDetails = new List<FoodReportDetailsViewModel>()
            };
            foodReportViewModel.FoodOrdersDetails = await foodListingService.GetOrderReportByDate(orderDate, "1", cancellationToken);

            return View(foodReportViewModel);
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
            var feedbackList = await _orderingService.GetFeedbackList(cancellationToken, page, pageSize);

            // Calculate pagination values
            var totalItems = feedbackList.Item2;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedItems = feedbackList.Item1; //feedbackList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
