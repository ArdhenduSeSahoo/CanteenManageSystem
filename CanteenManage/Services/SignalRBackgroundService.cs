using CanteenManage.Controllers;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace CanteenManage.Services
{
    public class SignalRBackgroundService : BackgroundService
    {
        private readonly SignalRDataHolder signalRDataHolder;
        private readonly IHubContext<OrderingHub> hubContext;
        public SignalRBackgroundService(SignalRDataHolder signalRDataHolder, IHubContext<OrderingHub> hubContext)
        {
            this.signalRDataHolder = signalRDataHolder;
            this.hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool sendNullOrderOnce = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Check if there are any orders in the list and if they are older than 5 seconds
                if (signalRDataHolder.GetOrderList().Count > 0)
                {
                    foreach (var order in signalRDataHolder.GetOrderList())
                    {

                        if (DateTime.Now.Subtract(order.RequestDateTime).Seconds > 5)
                        {
                            signalRDataHolder.RemoveOrderRequest(order);
                        }
                    }
                }

                if (signalRDataHolder.GetCanteenEmployeeList().Count > 0)
                {
                    if (sendNullOrderOnce || signalRDataHolder.GetOrderList().Count > 0)
                    {
                        // Notify all connected canteen employees about the current order list
                        await hubContext.Clients.Clients(signalRDataHolder.GetCanteenEmployeeList())
                            .SendAsync("OrderCompleteNotification", JsonConvert.SerializeObject(signalRDataHolder.GetOrderList()));
                        sendNullOrderOnce = false; // Reset the flag after sending the notification
                    }
                    //// Notify all connected canteen employees about the current order list
                    //await hubContext.Clients.Clients(signalRDataHolder.GetCanteenEmployeeList())
                    //    .SendAsync("OrderCompleteNotification", JsonConvert.SerializeObject(signalRDataHolder.GetOrderList()));

                    // once null data has been send then we set flag to false for next iteration
                    sendNullOrderOnce = signalRDataHolder.GetOrderList().Count > 0;
                }
                else
                {
                    // Optionally, you can handle the case where there are no canteen employees connected
                    // await hubContext.Clients.All.SendAsync("NoCanteenEmployeesConnected", "No canteen employees are currently available to complete the order.");
                }
                //Console.WriteLine("SignalRBackgroundService is running...");
                await Task.Delay(2000, stoppingToken); // Wait for 5 seconds before checking again
            }

            // This method is intentionally left empty as the service does not perform any background tasks.
            //await Task.CompletedTask;
        }
    }


}
