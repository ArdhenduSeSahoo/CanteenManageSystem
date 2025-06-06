using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CommitteeMembers
{
    public class EmployeeOrderReportController : Controller
    {
        private readonly FoodListingService foodListingService;

        public EmployeeOrderReportController(FoodListingService foodListingService)
        {
            this.foodListingService = foodListingService;
        }
        public async Task<IActionResult> EmpOrderReport(CancellationToken cancellationToken, int? month, int? year)
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
        public async Task<IActionResult> EmployeeOrderReportDetails(DateTime orderDate, CancellationToken cancellationToken)
        {
            var data = await foodListingService.GetCanteenOrderReportDataByDateRange(orderDate, cancellationToken);
            return View(data);
        }

    }
}
