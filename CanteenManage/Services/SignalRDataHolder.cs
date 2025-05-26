using CanteenManage.Models;
using System.Collections.Concurrent;

namespace CanteenManage.Services
{
    public class SignalRDataHolder
    {
        public static ConcurrentBag<string> canteenEmpList = new ConcurrentBag<string>();
        public static ConcurrentBag<SROrderModel> RequestOrderList = new ConcurrentBag<SROrderModel>();
        public static bool OrderListFilteringStatus { get; set; } = false;

        public SignalRDataHolder()
        {

        }
        public void AddCanteenEmployee(string connectionId)
        {
            if (!string.IsNullOrEmpty(connectionId))
            {
                canteenEmpList.Add(connectionId);
            }
        }
        public void AddOrderRequest(SROrderModel order)
        {
            if (order != null)
            {
                RequestOrderList.Add(order);
            }
        }
        public List<string> GetCanteenEmployeeList()
        {
            return canteenEmpList.ToList<string>(); // Return a list of connected canteen employees
        }
        public List<SROrderModel> GetOrderList()
        {
            return RequestOrderList.ToList<SROrderModel>(); // Return a list of connected canteen employees
        }
        public void ClearOrderList()
        {
            RequestOrderList.Clear();
            //OrderListFilteringStatus = false; // Reset filtering status
        }
        public void StartFilteringOrderList()
        {
            if (!OrderListFilteringStatus)
            {
                OrderListFilteringStatus = true; // Ensure filtering status is set to true
                // Logic to start filtering the order list can be added here
            }
        }
        public void StopFilteringOrderList()
        {
            OrderListFilteringStatus = false; // Reset filtering status
            // Logic to stop filtering the order list can be added here
        }
        public void RemoveCanteenEmployee(string connectionId)
        {
            if (!string.IsNullOrEmpty(connectionId) && canteenEmpList.Contains(connectionId))
            {
                canteenEmpList.TryTake(out connectionId);
            }
        }
        public void RemoveOrderRequest(SROrderModel order)
        {
            if (order != null)
            {
                RequestOrderList.TryTake(out order);
            }
        }
        public void RemoveOrderRequest(string orderid)
        {
            if (!string.IsNullOrEmpty(orderid))
            {
                var foundorder = RequestOrderList.Where(x => x.OrderId == orderid).FirstOrDefault();//ToList().ForEach(x => RequestOrderList.TryTake(out x));
                RequestOrderList.TryTake(out foundorder);
            }
        }
        public void ClearCanteenEmployeeList()
        {
            canteenEmpList.Clear();
        }


    }
}
