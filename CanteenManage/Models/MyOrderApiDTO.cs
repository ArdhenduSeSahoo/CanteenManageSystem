namespace CanteenManage.Models
{
    public class MyOrderApiDTO
    {
        public string foodOrderId { get; set; }
        public string orderid { get; set; }
    }
    public class CartRemoveItemApiDTO
    {
        public string OrderId { get; set; }
    }

}
