using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class CartController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly OrderingService orderingService;
        private readonly CartService cartService;
        public CartController(CanteenManageDBContext canteenManageContext, OrderingService ordering, CartService cartService)
        {
            this.canteenManageContext = canteenManageContext;
            this.orderingService = ordering;
            this.cartService = cartService;
        }
        public async Task<IActionResult> CartIndex(CancellationToken cancellationToken)
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            CartViewDataModel cartViewDataModel = new CartViewDataModel();
            try
            {
                SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
                var breakfastCart = await cartService.getCartList((int)FoodTypeEnum.Breakfast,
                    sessionDataModel.UserId,
                    cancellationToken
                    );
                var lunchCart = await cartService.getCartList((int)FoodTypeEnum.Lunch,
                    sessionDataModel.UserId,
                    cancellationToken
                    );
                var snaksCart = await cartService.getCartList((int)FoodTypeEnum.Snacks,
                    sessionDataModel.UserId,
                    cancellationToken
                    );
                var outofDateList = await cartService.getCartOutDateList(sessionDataModel.UserId, cancellationToken);


                cartViewDataModel.BreakFastFoodOrders = breakfastCart;
                cartViewDataModel.LunchFoodOrders = lunchCart;
                cartViewDataModel.SnaksFoodOrders = snaksCart;
                cartViewDataModel.OutOfStockOrders = outofDateList;
            }
            catch (Exception ex)
            {

            }
            return View(cartViewDataModel);
        }
        public async Task<IActionResult> RemoveOrder(IFormCollection formcollect, CancellationToken cancellationToken)
        {
            var orderId = formcollect["orderId"];
            var orderType = formcollect["orderType"];
            try
            {


                if (!string.IsNullOrEmpty(orderId))
                {
                    SessionDataModel sessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
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
        public async Task<IActionResult> PlaceOrder(CancellationToken cancellationToken)//IFormCollection formcollect,
        {
            SessionDataModel SessionDataModel = SessionDataHelper.GetSessionDataModel(HttpContext.Session);
            await cartService.PlaceOrder(sessionData: SessionDataModel, cancellationToken: cancellationToken);
            return this.RedirectToAction("CartIndex");
        }
    }
}
