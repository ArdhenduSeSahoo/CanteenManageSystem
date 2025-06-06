using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class CartController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        private readonly CartService cartService;
        private readonly UtilityServices utilityServices;
        public CartController(CanteenManageDBContext canteenManageContext, OrderingService ordering, CartService cartService, UtilityServices utilityServices)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = ordering;
            this.cartService = cartService;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> CartIndex(CancellationToken cancellationToken)
        {
            CartViewDataModel cartViewDataModel = new CartViewDataModel();
            try
            {
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                var breakfastCart = await cartService.getCartList((int)FoodTypeEnum.Breakfast,
                    sessionDataModel.UserIdOrZero,
                    cancellationToken
                    );
                var lunchCart = await cartService.getCartList((int)FoodTypeEnum.Lunch,
                    sessionDataModel.UserIdOrZero,
                    cancellationToken
                    );
                var snaksCart = await cartService.getCartList((int)FoodTypeEnum.Snacks,
                    sessionDataModel.UserIdOrZero,
                    cancellationToken
                    );
                var outofDateList = await cartService.getCartOutDateList(sessionDataModel.UserIdOrZero, cancellationToken);
                var existingorders = await cartService.getCartItemInOrderList(sessionDataModel.UserIdOrZero, cancellationToken);

                cartViewDataModel.BreakFastFoodOrders = breakfastCart;
                cartViewDataModel.LunchFoodOrders = lunchCart;
                cartViewDataModel.SnaksFoodOrders = snaksCart;
                cartViewDataModel.OutOfStockOrders = outofDateList;
                cartViewDataModel.CartItemInOrders = existingorders;
            }
            catch (Exception ex)
            {

            }
            return View(cartViewDataModel);
        }

        [HttpPost]
        public async Task<IResult> RemoveItem([FromBody] CartRemoveItemApiDTO cartRemoveItemApiDTO, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(cartRemoveItemApiDTO.OrderId))
                {
                    SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                    //if (!string.IsNullOrEmpty(orderType))
                    //{
                    //    await cartService.ClearCart(sessionDataModel, int.Parse(orderId), cancellationToken, int.Parse(orderType));
                    //}
                    //else
                    {
                        var retval = await cartService.RemoveCartItem(sessionDataModel, int.Parse(cartRemoveItemApiDTO.OrderId), cancellationToken);
                        if (retval)
                        {
                            return Results.Ok(new { isDeleted = "ok" });
                        }
                        else
                        {
                            return Results.Ok(new { isDeleted = "no" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return Results.Ok(new { isDeleted = "no" });
            }


            return Results.Ok(new { isDeleted = "no" });
        }
        public async Task<IActionResult> RemoveOrder(IFormCollection formcollect, CancellationToken cancellationToken)
        {
            var orderId = formcollect["orderId"];
            var orderType = formcollect["orderType"];
            try
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                    if (!string.IsNullOrEmpty(orderType))
                    {
                        await cartService.ClearCart(sessionDataModel, int.Parse(orderId), cancellationToken, int.Parse(orderType));
                    }
                    else
                    {
                        await cartService.ClearCart(sessionDataModel, int.Parse(orderId), cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {


            }

            return this.RedirectToAction("CartIndex");
        }

        [HttpPost]
        public IActionResult PlaceOrder(CancellationToken cancellationToken)//IFormCollection formcollect,
        {
            SessionDataModel SessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            try
            {
                cartService.PlaceOrder(sessionData: SessionDataModel, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index", "MyOrders"); //this.RedirectToAction("CartIndex");
        }
    }
}
