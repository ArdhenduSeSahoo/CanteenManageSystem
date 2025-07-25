using CanteenManage.CanteenRepository.Models;
using System.Threading;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace CanteenManage.Controllers.CanteenControllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class OrderListViewController : Controller
    {
        private readonly FoodListingService _foodListingService;
        private readonly ILogger<OrderListViewController> _logger;
        private readonly Channel<OrderConformingChanelRequest> _orderConformingChannel;
        public OrderListViewController(FoodListingService foodListingService, ILogger<OrderListViewController> logger, Channel<OrderConformingChanelRequest> channel)
        {
            this._foodListingService = foodListingService;
            _logger = logger;
            _orderConformingChannel = channel;
        }
        public async Task<IActionResult> Index(string FoodType, CancellationToken cancellationToken, string searchTerm = "")
        {
            return await getParticipialWithData(
                FoodType: FoodType,
                cancellationToken: cancellationToken,
                searchTerm: searchTerm
                );
        }

        public async Task<IActionResult> CompleteFoodOrder(string foodorderid, string foodtype, CancellationToken cancellationToken, string searchTerm = "")
        {
            if (string.IsNullOrEmpty(foodtype))
            {
                foodtype = "1";
            }
            try
            {
                await _foodListingService.CompleteFoodOrder(foodorderid);
                await _orderConformingChannel.Writer.WriteAsync(
                    new OrderConformingChanelRequest
                    {
                        OrderId = foodorderid,
                        FoodType = Convert.ToInt32(foodtype)
                    },
                    cancellationToken
                );
            }
            catch (Exception ex)
            {

            }

            return await getParticipialWithData(
                FoodType: foodtype,
                cancellationToken: cancellationToken,
                searchTerm: searchTerm
                );

        }

        public async Task<IActionResult> getParticipialWithData(string FoodType, CancellationToken cancellationToken, string searchTerm = "")
        {
            FoodTypeEnum foodType = FoodTypeEnum.Breakfast;
            List<EmployeeFoodOrdersTableDataModel> foodOrders = new List<EmployeeFoodOrdersTableDataModel>();
            OrderListViewTodaysDataModel orderListViewTodaysDataModel = new OrderListViewTodaysDataModel();
            orderListViewTodaysDataModel.EmployeeFoodOrdersTableData = foodOrders;
            orderListViewTodaysDataModel.FoodType = 1; // Default to Breakfast
            try
            {
                if (string.IsNullOrEmpty(FoodType))
                {
                    FoodType = Convert.ToString((int)FoodTypeEnum.Breakfast) ?? "1";
                }
                if (!string.IsNullOrEmpty(FoodType))
                {

                    orderListViewTodaysDataModel.FoodType = Convert.ToInt32(FoodType) > 0 ? Convert.ToInt32(FoodType) : 1;

                    var foodtype_qp = Convert.ToInt32(FoodType);
                    foodType = (FoodTypeEnum)foodtype_qp;
                    //if (foodtype_qp < 1)
                    //{
                    //    //searchTerm = searchTerm.Trim().ToLower();
                    //    //foodOrders = await foodListingService.GetFoodOrdersOld_CU(cancellationToken, searchTerm);
                    //}
                    //if (string.IsNullOrWhiteSpace(searchTerm))
                    //{
                    //    foodOrders = await _foodListingService.GetFoodOrdersToday(foodType, cancellationToken);
                    //}
                    //else
                    {
                        searchTerm = searchTerm.Trim().ToLower();
                        foodOrders = await _foodListingService.GetFoodOrdersToday_Filter(foodType, cancellationToken, searchTerm);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request in OrderListViewController.");
            }
            orderListViewTodaysDataModel.EmployeeFoodOrdersTableData = foodOrders;
            return PartialView(CustomDataConstants.CustomViewPath + "\\_OrderListViewTodays", orderListViewTodaysDataModel);
        }

    }
}
