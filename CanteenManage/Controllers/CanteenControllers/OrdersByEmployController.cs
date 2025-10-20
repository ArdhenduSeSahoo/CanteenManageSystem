using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Azure.Core;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Models.DTO;
using CanteenManage.Services;
using CanteenManage.Utility;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CanteenManage.Controllers.CanteenControllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class OrdersByEmployController : Controller
    {
        private readonly FoodListingService foodListingService;
        //private readonly UtilityServices utilityServices;
        private readonly ILogger<OrdersByEmployController> logger;
        private readonly Channel<OrderConformingChanelRequest> _orderConformingChannel;
        public OrdersByEmployController(FoodListingService foodListingService, ILogger<OrdersByEmployController> logger, Channel<OrderConformingChanelRequest> channel)
        {
            this.foodListingService = foodListingService;
            this.logger = logger;
            //this.utilityServices = utilityServices;
            _orderConformingChannel = channel;

        }
        public async Task<IActionResult> OrderByEmployIdx(string FoodType, CancellationToken cancellationToken, string searchTerm = "")
        {
            FoodTypeEnum foodType = FoodTypeEnum.Breakfast;
            List<EmployeeFoodOrdersTableDataModel> foodOrders = new List<EmployeeFoodOrdersTableDataModel>();
            OrderByEmployViewDataModel model = new OrderByEmployViewDataModel();
            //string searchTerm = string.Empty;
            model.screenTitle = "Orders By Employee";
            model.FoodOrders = foodOrders;

            try
            {
                if (string.IsNullOrEmpty(FoodType))
                {
                    FoodType = Convert.ToString((int)FoodTypeEnum.Breakfast);
                    //throw new ArgumentException("FoodType cannot be null or empty. Defaulting to Breakfast.");
                }
                if (!string.IsNullOrEmpty(FoodType))
                {
                    var foodtype_qp = Convert.ToInt32(FoodType);
                    foodType = (FoodTypeEnum)foodtype_qp;
                    //if (foodtype_qp < 1)
                    //{
                    //    //searchTerm = searchTerm.Trim().ToLower();
                    //    //foodOrders = await foodListingService.GetFoodOrdersOld_CU(cancellationToken, searchTerm);
                    //}
                    if (string.IsNullOrWhiteSpace(searchTerm))
                    {
                        foodOrders = await foodListingService.GetFoodOrdersToday(foodType, cancellationToken);
                    }
                    else
                    {

                        searchTerm = searchTerm.Trim().ToLower();

                        foodOrders = await foodListingService.GetFoodOrdersToday_Filter(foodType, cancellationToken, searchTerm);
                    }
                }

                var screenTile = foodType switch
                {
                    FoodTypeEnum.Breakfast => "Breakfast Orders",
                    FoodTypeEnum.Lunch => "Lunch Orders",
                    FoodTypeEnum.Snacks => "Snacks Orders",
                    FoodTypeEnum.Dinner => "Dinner Orders",
                    _ => "Orders",
                };


                model.screenTitle = screenTile;
                model.FoodOrders = foodOrders;
                model.FoodType = (int)foodType;
                model.SearchValue = searchTerm;
            }
            catch (Exception ex)
            {
                // Optional: log error
                logger.LogError(ex, "An error occurred while processing the request in OrdersByEmployController.");
            }


            return View(model);
        }

        //public async Task<IActionResult> CompleteFoodOrder(IFormCollection formcollect)
        //{
        //    var foodOrderId = formcollect["foodId"];
        //    var foodtype = formcollect["foodtype"];
        //    if (string.IsNullOrEmpty(foodtype))
        //    {
        //        foodtype = "1";
        //    }
        //    try
        //    {
        //        await foodListingService.CompleteFoodOrder(foodOrderId);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error completing food order with ID: {FoodOrderId}", foodOrderId);
        //    }

        //    return this.RedirectToAction("OrderByEmployIdx", new { FoodType = foodtype });
        //}

        public async Task<IActionResult> GetSearchResult(string searchVal)
        {
            return PartialView("OrderByEmploySearchResult");
        }

        [HttpGet]
        public async Task<IActionResult> SearchOrders(string query)
        {
            var results = await foodListingService.SearchOrdersByEmployee(query);
            return View("OrderByEmployIdx", results);
        }

        public IActionResult OrderScanner()
        {
            return View();
        }
        [HttpPost]
        public async Task<IResult> SingleOrderDetailP([FromBody] SingleOrderDetainsDTO singleOrderDetaisDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(singleOrderDetaisDTO.orderdetails))
            {
                return Results.Ok(new { error = "Order data is null." });
            }
            else
            {
                try
                {
                    DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();

                    //encodedDetails = "5dKDT+UWEAIhDlcFzpjAD2Kz12hYMCvru9tIb0meoO8rNgJ0NHOX9ShSnU88yXehsPu0ltpxWAMxFjM1VgyN/A==";

                    var decodedDetails = new EncryptionDecryptions().DecryptString(singleOrderDetaisDTO.orderdetails);

                    var splittedDetails = decodedDetails.Split("-|-");
                    DateTime dateTimetoday = dateCalculationHelper.DateTimeFromString(splittedDetails[2]);
                    if (dateTimetoday.Date == DateTime.Now.Date)
                    {
                        var foodorder = await foodListingService.GetFoodOrdersToday_Single(
                            cancellationToken: cancellationToken,
                            EmpID: Convert.ToInt32(splittedDetails[1]),
                            OrderID: splittedDetails[0]
                            );
                        if (foodorder != null)
                        {
                            return Results.Ok(new { detail = foodorder, error = "" });
                        }
                        else
                        {
                            return Results.Ok(new { error = "Order data is not found." });
                        }
                    }
                    else
                    {
                        return Results.Ok(new { error = "Order data is not found." });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error decoding order details.");
                }

            }
            return Results.Ok(new { error = "Something went wrong." });
        }

        [HttpPost]
        public async Task<IResult> SingleOrderConform(CancellationToken cancellationToken, [FromBody] SingleOrderDetainsDTO singleOrderDetainsDTO)
        {
            string errormessage = "";
            if (string.IsNullOrEmpty(singleOrderDetainsDTO.orderdetails))
            {
                return Results.Ok(new { error = "Order data is null." });
            }
            else
            {
                try
                {
                    DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();
                    var decodedDetails = new EncryptionDecryptions().DecryptString(singleOrderDetainsDTO.orderdetails);
                    var splittedDetails = decodedDetails.Split("-|-");
                    DateTime dateTimetoday = dateCalculationHelper.DateTimeFromString(splittedDetails[2]);
                    if (dateTimetoday.Date == DateTime.Now.Date)
                    {
                        await _orderConformingChannel.Writer.WriteAsync(
                               new OrderConformingChanelRequest
                               {
                                   OrderId = splittedDetails[0],
                                   FoodType = null
                               },
                               cancellationToken
                           );

                        return Results.Ok(new { error = "" });
                    }
                    else
                    {
                        return Results.Ok(new { error = "Order not found." });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error decoding order details.");
                    errormessage = ex.Message;
                }

            }
            return Results.Ok(new { error = errormessage });
        }

    }

}