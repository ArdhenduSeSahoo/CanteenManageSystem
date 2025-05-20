using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    public class CanteenOrderReportController : Controller
    {
        private readonly FoodListingService foodListingService;

        public CanteenOrderReportController(FoodListingService foodListingService)
        {
            this.foodListingService = foodListingService;
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
    }
}
