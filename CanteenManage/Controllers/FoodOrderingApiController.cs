using System.Threading;
using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FoodOrderingAPIController : Controller
    {
        private readonly CartService cartService;
        private readonly UtilityServices _utilityServices;
        private readonly ILogger<FoodOrderingAPIController> logger;
        public FoodOrderingAPIController(CartService cartService, UtilityServices utilityServices, ILogger<FoodOrderingAPIController> logger)
        {
            this.cartService = cartService;
            this._utilityServices = utilityServices;
            this.logger = logger;
        }
        public IResult Index()
        {
            return Results.Ok(new { message = "This is a  AIP." });
        }
        public async Task<IResult> AddBreakFastFoodOrder(string? foodid, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                if (string.IsNullOrEmpty(foodid))
                {
                    return Results.Ok(new { error = "Food ID is required." });
                }
                if (sessionDataModel.UserSelectedDay == null)
                {
                    return Results.Ok(new { error = "User selected day is not set." });
                }
                int foodIdInt = int.Parse(foodid);
                var isFoodAvailableForBook = await cartService.ValidateFoodForSelectedDate(FoodTypeEnum.Breakfast, foodIdInt, sessionDataModel, cancellationToken: cancellationToken);
                if (!isFoodAvailableForBook)
                {
                    return Results.Ok(new { error = "Food is not available for the selected date." });
                }
                else
                {
                    var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Breakfast,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: new FoodOrdersFormBodyModel() { FoodOrderId = foodIdInt.ToString() },
                        cancellationToken: cancellationToken
                      );
                    return orderResult;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddBreakFastFoodOrder method: " + ex.Message);
            }
            return Results.Ok(new { error = "Some Error found." });
        }
        public async Task<IResult> AddLunchFoodOrder(string? foodid, CancellationToken cancellationToken)
        {

            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                if (string.IsNullOrEmpty(foodid))
                {
                    return Results.Ok(new { error = "Food ID is required." });
                }
                if (sessionDataModel.UserSelectedDay == null)
                {
                    return Results.Ok(new { error = "User selected day is not set." });
                }
                int foodIdInt = int.Parse(foodid);
                var isFoodAvailableForBook = await cartService.ValidateFoodForSelectedDate(FoodTypeEnum.Lunch, foodIdInt, sessionDataModel, cancellationToken: cancellationToken);
                if (!isFoodAvailableForBook)
                {
                    return Results.Ok(new { error = "Food is not available for the selected date." });
                }
                else
                {
                    var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Lunch,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: new FoodOrdersFormBodyModel() { FoodOrderId = foodIdInt.ToString() },
                        cancellationToken: cancellationToken
                      );
                    return orderResult;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddBreakFastFoodOrder method: " + ex.Message);
            }
            return Results.Ok(new { error = "Some Error found." });
        }
        public async Task<IResult> AddSnacksFoodOrder(string? foodid, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                if (string.IsNullOrEmpty(foodid))
                {
                    return Results.Ok(new { error = "Food ID is required." });
                }
                if (sessionDataModel.UserSelectedDay == null)
                {
                    return Results.Ok(new { error = "User selected day is not set." });
                }
                int foodIdInt = int.Parse(foodid);
                var isFoodAvailableForBook = await cartService.ValidateFoodForSelectedDate(FoodTypeEnum.Snacks, foodIdInt, sessionDataModel, cancellationToken: cancellationToken);
                if (!isFoodAvailableForBook)
                {
                    return Results.Ok(new { error = "Food is not available for the selected date." });
                }
                else
                {
                    var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Snacks,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: new FoodOrdersFormBodyModel() { FoodOrderId = foodIdInt.ToString() },
                        cancellationToken: cancellationToken
                      );
                    return orderResult;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddBreakFastFoodOrder method: " + ex.Message);
            }
            return Results.Ok(new { error = "Some Error found." });
        }

        public async Task<IResult> AddDinnerFoodOrder(string? foodid, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                if (string.IsNullOrEmpty(foodid))
                {
                    return Results.Ok(new { error = "Food ID is required." });
                }
                if (sessionDataModel.UserSelectedDay == null)
                {
                    return Results.Ok(new { error = "User selected day is not set." });
                }
                int foodIdInt = int.Parse(foodid);
                var isFoodAvailableForBook = await cartService.ValidateFoodForSelectedDate(FoodTypeEnum.Dinner, foodIdInt, sessionDataModel, cancellationToken: cancellationToken);
                if (!isFoodAvailableForBook)
                {
                    return Results.Ok(new { error = "Food is not available for the selected date." });
                }
                else
                {
                    var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Dinner,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: new FoodOrdersFormBodyModel() { FoodOrderId = foodIdInt.ToString() },
                        cancellationToken: cancellationToken
                      );
                    return orderResult;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddBreakFastFoodOrder method: " + ex.Message);
            }
            return Results.Ok(new { error = "Some Error found." });
        }

        public bool isFoodAvailableForBook(CancellationToken cancellationToken, SessionDataModel sessionDataModel, FoodTypeEnum foodTypeEnum)
        {
            //try
            //{
            //    SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            //    if (string.IsNullOrEmpty(foodid))
            //    {
            //        return Results.Ok(new { error = "Food ID is required." });
            //    }
            //    if (sessionDataModel.UserSelectedDay == null)
            //    {
            //        return Results.Ok(new { error = "User selected day is not set." });
            //    }
            //    int foodIdInt = int.Parse(foodid);
            //    var isFoodAvailableForBook = await cartService.ValidateFoodForSelectedDate(FoodTypeEnum.Breakfast, foodIdInt, sessionDataModel, cancellationToken: cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "Error in AddBreakFastFoodOrder method: " + ex.Message);
            //}
            return true; // Placeholder for actual implementation
        }
        [HttpPost("breakfastFoodOrderAdd")]
        public async Task<IResult> breakfastFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);

                //var orderResult = await orderingService.AddFoodOrder(
                //      foodTypeEnum: FoodTypeEnum.Breakfast,
                //      sessionData: sessionDataModel,
                //        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                //        cancellationToken: cancellationToken
                //      );
                var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Breakfast,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                        cancellationToken: cancellationToken
                      );
                return orderResult;

            }
            catch (Exception ex)
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "Some error happening. please try after sometimes.",
                });
            }
            return Results.Ok(new FoodOrderApiReturnMessage()
            {
                error = "Some error happening.",
            });
        }
        [HttpPost("LunchFoodOrderAdd")]
        public async Task<IResult> LunchFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                //var orderResult = await orderingService.AddFoodOrder(
                //      foodTypeEnum: FoodTypeEnum.Lunch,
                //      sessionData: sessionDataModel,
                //        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                //        cancellationToken: cancellationToken
                //      );
                var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Lunch,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                        cancellationToken: cancellationToken
                      );
                return orderResult;

            }
            catch (Exception ex)
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "Some error happening. please try after sometimes.",
                });
            }
            return Results.Ok(new FoodOrderApiReturnMessage()
            {
                error = "Some error happening.",
            });
        }

        [HttpPost("SnacksFoodOrderAdd")]
        public async Task<IResult> SnacksFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                //var orderResult = await orderingService.AddFoodOrder(
                //      foodTypeEnum: FoodTypeEnum.Snacks,
                //      sessionData: sessionDataModel,
                //        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                //        cancellationToken: cancellationToken
                //      );
                var orderResult = await cartService.AddToCart(
                      foodTypeEnum: FoodTypeEnum.Snacks,
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                        cancellationToken: cancellationToken
                      );
                return orderResult;

            }
            catch (Exception ex)
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "Some error happening. please try after sometimes.",
                });
            }
            return Results.Ok(new FoodOrderApiReturnMessage()
            {
                error = "Some error happening.",
            });
        }
        public async Task<IResult> OrderRemove(string? foodid, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                var order_remove_result = await cartService.RemoveFromCart(
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: new FoodOrdersFormBodyModel() { FoodOrderId = foodid },
                        cancellationToken: cancellationToken
                      );
                return order_remove_result;

            }
            catch (Exception ex)
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "Some error happening. please try after sometimes.",
                });
            }
            return Results.Ok(new FoodOrderApiReturnMessage()
            {
                error = "Some error happening.",
            });
        }

        [HttpPost("OrderRemove")]
        public async Task<IResult> OrderRemove([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = _utilityServices.GetSessionDataModel(HttpContext.Session);
                var order_remove_result = await cartService.RemoveFromCart(
                      sessionData: sessionDataModel,
                        foodOrdersFormBodyModel: foodOrdersFormBodyModel,
                        cancellationToken: cancellationToken
                      );
                return order_remove_result;

            }
            catch (Exception ex)
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "Some error happening. please try after sometimes.",
                });
            }
            return Results.Ok(new FoodOrderApiReturnMessage()
            {
                error = "Some error happening.",
            });
        }
    }
}
