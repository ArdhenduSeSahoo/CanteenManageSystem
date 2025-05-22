using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    public class CanteenOrderReportController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly OrderingService _orderingService;

        public CanteenOrderReportController(FoodListingService foodListingService, OrderingService orderingService)
        {
            this.foodListingService = foodListingService;
            _orderingService = orderingService;
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
            var feedbackList = await _orderingService.GrtFeedbackList(cancellationToken);

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
    }
}
