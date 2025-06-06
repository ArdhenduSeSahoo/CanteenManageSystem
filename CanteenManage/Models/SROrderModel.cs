namespace CanteenManage.Models
{
    public class SROrderModel
    {
        public string OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderQnt { get; set; }
        public string UserEmpId { get; set; }
        public string UserName { get; set; }
        public string ConnectionID { get; set; } // This is the connection ID of the client making the request
        public DateTime RequestDateTime { get; set; }
    }
}
