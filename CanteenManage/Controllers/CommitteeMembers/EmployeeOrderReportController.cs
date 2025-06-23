using System.Data;
using System.Globalization;
using System.Threading;
using CanteenManage.Models;
using CanteenManage.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CanteenManage.Controllers.CommitteeMembers
{
    [Authorize(Roles = "CommitteeMember")]
    public class EmployeeOrderReportController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly OrderingService _orderingService;

        public EmployeeOrderReportController(FoodListingService foodListingService, OrderingService orderingService)
        {
            this.foodListingService = foodListingService;
            _orderingService = orderingService;
        }
        public async Task<IActionResult> EmpOrderReport(CancellationToken cancellationToken, string? fromDatePicker, string? toDatePicker, string? orderStatusOptions)
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

        public async Task<IActionResult> OrderFeedback(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var feedbackList = await _orderingService.GetFeedbackList(cancellationToken, page, pageSize);

            // Calculate pagination values
            var totalItems = feedbackList.Item2;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedItems = feedbackList.Item1;//feedbackList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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

    }
}
