using System.Collections.Concurrent;
using CanteenManage.Models;

namespace CanteenManage.Services
{
    public class OrderDataCaching
    {

        public ConcurrentDictionary<string, EmployeeFoodOrdersTableDataModel> OrderCacheDataDictionary;
        public OrderDataCaching()
        {

            OrderCacheDataDictionary = new ConcurrentDictionary<string, EmployeeFoodOrdersTableDataModel>();
        }
    }
}
