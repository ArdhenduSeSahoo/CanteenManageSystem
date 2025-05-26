using System.Collections.Concurrent;
using CanteenManage.Models;
using CanteenManage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CanteenManage.Controllers
{
    //[Authorize]
    public class OrderingHub : Hub
    {
        //public static ConcurrentBag<string> canteenEmpList = new ConcurrentBag<string>();
        //public static ConcurrentBag<SROrderModel> RequestOrderList = new ConcurrentBag<SROrderModel>();
        //public static bool OrderListFilteringStatus = false;
        private readonly SignalRDataHolder signalRDataHolder;
        public readonly FoodListingService foodListingService;
        public OrderingHub(SignalRDataHolder signalRDataHolder, FoodListingService foodListingService)
        {
            this.signalRDataHolder = signalRDataHolder;
            this.foodListingService = foodListingService;
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task RequestForOrderComplete(string OrderId, string ordername, string orderqnt, string userempid, string username)
        {
            SROrderModel order = new SROrderModel
            {
                OrderId = OrderId,
                OrderName = ordername,
                OrderQnt = orderqnt,
                UserEmpId = userempid,
                UserName = username,
                RequestDateTime = DateTime.Now
            };
            //RequestOrderList.Add(order);
            signalRDataHolder.AddOrderRequest(order);


            if (signalRDataHolder.GetOrderList().Count > 0)
            {
                await Clients.Clients(signalRDataHolder.GetCanteenEmployeeList())
                        .SendAsync("OrderCompleteNotification", signalRDataHolder.GetOrderList());
                // await Clients.Clients(canteenEmpList).SendAsync("OrderCompleteNotification", JsonConvert.SerializeObject(RequestOrderList));
            }
            else
            {
                // Optionally, you can handle the case where there are no canteen employees connected
                //await Clients.Caller.SendAsync("NoCanteenEmployeesConnected", "No canteen employees are currently available to complete the order.");
            }
        }

        public async Task AssignAsCanteenEmployee(string message)
        {
            signalRDataHolder.AddCanteenEmployee(Context.ConnectionId ?? string.Empty);

            //StartFilteringOrderList();
        }

        public async Task ConformingOrder(string orderId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(orderId) && !string.IsNullOrEmpty(orderId))
                {
                    //await foodListingService.CompleteFoodOrder(int.Parse(orderId));
                    signalRDataHolder.RemoveOrderRequest(orderId);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public async Task SendNotification(string user, string notification)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", notification);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            // Optional: Add logic for when a user connects
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            signalRDataHolder.RemoveCanteenEmployee(Context.ConnectionId ?? string.Empty);
            //canteenEmpList.TryTake(out string? connectionId);
            //if (connectionId != null)
            //{
            //    // Optionally, you can notify other users that this user has disconnected
            //    await Clients.All.SendAsync("UserDisconnected", connectionId);
            //}
            // Optional: Add logic for when a user disconnects
        }
    }
}
