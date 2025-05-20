namespace CanteenManage.Models
{
    public class CanteenOrderReportViewDataModel
    {
        public string ReportForMonthName { get; set; }
        public List<ReportMonthsDDLDataModel> ReportMonthsDDLDataModel { get; set; }
        public List<CanteenOrdersReportTableViewDataModel> canteenOrdersReportTableViewDataModels { get; set; }
    }

    public class ReportMonthsDDLDataModel
    {
        public string DDL_Id { get; set; }
        public string Values { get; set; }
    }

    public class CanteenOrdersReportTableViewDataModel
    {
        public DateTime OrderDate { get; set; }
        public int TotalOrderCount { get; set; }
        public int TotalEmployeeCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalEmployeePrice { get; set; }
        public decimal TotalSubsidyPrice { get; set; }

    }
}
