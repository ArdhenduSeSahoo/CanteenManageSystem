using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    public class OrdersByEmployController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        public OrdersByEmployController(FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> OrderByEmployIdx(string FoodType, CancellationToken cancellationToken)
        {
            FoodTypeEnum foodType = FoodTypeEnum.Breakfast;
            if (!string.IsNullOrEmpty(FoodType))
            {
                try
                {
                    var foodtype_qp = Convert.ToInt32(FoodType);
                    if (foodtype_qp == (int)FoodTypeEnum.Breakfast)
                    {
                        foodType = FoodTypeEnum.Breakfast;
                    }
                    else if (foodtype_qp == (int)FoodTypeEnum.Lunch)
                    {
                        foodType = FoodTypeEnum.Lunch;
                    }
                    else if (foodtype_qp == (int)FoodTypeEnum.Snacks)
                    {
                        foodType = FoodTypeEnum.Snacks;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            var foodlist = await foodListingService.GetFoodOrdersToday(foodType, cancellationToken);
            var screenTile = foodType switch
            {
                FoodTypeEnum.Breakfast => "Breakfast Orders",
                FoodTypeEnum.Lunch => "Lunch Orders",
                FoodTypeEnum.Snacks => "Snacks Orders",
                _ => "Orders",
            };

            OrderByEmployViewDataModel orderByEmployViewDataModel = new OrderByEmployViewDataModel();
            orderByEmployViewDataModel.screenTitle = screenTile;
            orderByEmployViewDataModel.FoodOrders = foodlist;
            orderByEmployViewDataModel.FoodType = (int)foodType;
            return View(orderByEmployViewDataModel);
        }

        public async Task<IActionResult> OrderByEmployBreakfast()
        {
            return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "1" });
        }
        public async Task<IActionResult> OrderByEmployLunch()
        {
            return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "2" });
        }
        public async Task<IActionResult> OrderByEmploySnacks()
        {
            return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "3" });
        }
        public async Task<IActionResult> CompleteFoodOrder(IFormCollection formcollect)
        {
            var foodOrderId = formcollect["foodId"];
            var foodtype = formcollect["foodtype"];
            if (string.IsNullOrEmpty(foodtype))
            {
                foodtype = "1";
            }
            try
            {
                await foodListingService.CompleteFoodOrder(Convert.ToInt32(foodOrderId));
            }
            catch (Exception ex)
            {

            }

            return this.RedirectToAction("OrderByEmployIdx", new { FoodType = foodtype });
        }

        public async Task<IActionResult> GetSearchResult(string searchVal)
        {
            return PartialView("OrderByEmploySearchResult");
        }

    }
}
