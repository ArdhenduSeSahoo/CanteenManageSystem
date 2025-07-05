using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace CanteenManage.Services
{
    public class OrderProcessingBackgroundServices : BackgroundService
    {
        private readonly ILogger<OrderProcessingBackgroundServices> _logger;
        private readonly OrderDataCaching _orderDataCaching;
        private readonly Channel<OrderConformingChanelRequest> _orderConformingChannel;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrderProcessingBackgroundServices(ILogger<OrderProcessingBackgroundServices> logger, OrderDataCaching orderDataCaching, Channel<OrderConformingChanelRequest> channel, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _orderDataCaching = orderDataCaching;
            _orderConformingChannel = channel;
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _orderConformingChannel.Reader.WaitToReadAsync(stoppingToken))
            {
                try
                {
                    _orderConformingChannel.Reader.TryRead(out OrderConformingChanelRequest? request);
                    if (request != null)
                    {
                        using (IServiceScope serviceScope = _serviceScopeFactory.CreateAsyncScope())
                        {
                            CanteenManageDBContext contextCM = serviceScope.ServiceProvider.GetRequiredService<CanteenManageDBContext>();
                            try
                            {
                                await contextCM.FoodOrders.Where(fo => fo.OrderID == request.OrderId)
                                                            .ExecuteUpdateAsync(fo => fo.SetProperty(f => f.IsCompleted, true));
                                await contextCM.SaveChangesAsync();

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while updating the food order with ID: {OrderId}", request.OrderId);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Received a null request from the channel.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the background service.");
                }
            }
        }
    }
}
