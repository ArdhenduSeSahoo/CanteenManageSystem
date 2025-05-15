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

        public FoodOrdersController(CanteenManageDBContext _context, OrderingService orderingService)
        {
            this.context = _context;
            this.orderingService = orderingService;
        }

        [HttpPost("breakfastFoodOrderAdd")]
        public async Task<IResult> breakfastFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                var orderResult = await orderingService.AddFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Breakfast,
                      session: HttpContext.Session,
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

        [HttpPost("breakfastFoodOrderRemove")]
        public async Task<IResult> breakfastFoodOrderRemove([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {
            try
            {

                var order_remove_result = await orderingService.RemoveFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Breakfast,
                      session: HttpContext.Session,
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


        [HttpPost("LunchFoodOrderAdd")]
        public async Task<IResult> LunchFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                var orderResult = await orderingService.AddFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Lunch,
                      session: HttpContext.Session,
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

        [HttpPost("LunchFoodOrderRemove")]
        public async Task<IResult> LunchFoodOrderRemove([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {
            try
            {

                var order_remove_result = await orderingService.RemoveFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Lunch,
                      session: HttpContext.Session,
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


        [HttpPost("SnacksFoodOrderAdd")]
        public async Task<IResult> SnacksFoodOrderAdd([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {

            try
            {
                var orderResult = await orderingService.AddFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Snacks,
                      session: HttpContext.Session,
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

        [HttpPost("SnacksFoodOrderRemove")]
        public async Task<IResult> SnacksFoodOrderRemove([FromBody] FoodOrdersFormBodyModel foodOrdersFormBodyModel, CancellationToken cancellationToken)
        {
            try
            {

                var order_remove_result = await orderingService.RemoveFoodOrder(
                      foodTypeEnum: FoodTypeEnum.Snacks,
                      session: HttpContext.Session,
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
