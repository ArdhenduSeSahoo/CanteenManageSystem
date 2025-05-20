using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CanteenManage.Controllers.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodOrdersController : ControllerBase
    {
        private readonly CanteenManageDBContext context;
        private readonly OrderingService orderingService;
        private readonly CartService cartService;

        public FoodOrdersController(CanteenManageDBContext _context, OrderingService orderingService, CartService cartService)
        {
            this.context = _context;
            this.orderingService = orderingService;
            this.cartService = cartService;
        }

        [HttpPost("breakfastFoodOrderAdd")]
        public async Task<IResult> breakfastFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);

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
                SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
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
                SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
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


        [HttpPost("OrderRemove")]
        public async Task<IResult> OrderRemove([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {
            try
            {
                SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
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
