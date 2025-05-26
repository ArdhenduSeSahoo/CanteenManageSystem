namespace CanteenManage.Models
{
    public class SROrderModel
    {
        public string OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderQnt { get; set; }
        public string UserEmpId { get; set; }
        public string UserName { get; set; }

        public DateTime RequestDateTime { get; set; }
    }
}
