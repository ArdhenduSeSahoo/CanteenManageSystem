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
        public async Task<IActionResult> OrderByEmployIdx(string FoodType, CancellationToken cancellationToken, string searchTerm = "")
        {
            FoodTypeEnum foodType = FoodTypeEnum.Breakfast;
            if (!string.IsNullOrEmpty(FoodType))
            {
                try
                {
                    var foodtype_qp = Convert.ToInt32(FoodType);
                    foodType = (FoodTypeEnum)foodtype_qp;
                }
                catch (Exception ex)
                {
                    // Optional: log error
                }
            }
            searchTerm = searchTerm.Trim().ToLower();

            var foodlist = await foodListingService.GetFoodOrdersToday(foodType, cancellationToken, searchTerm);



            var screenTile = foodType switch
            {
                FoodTypeEnum.Breakfast => "Breakfast Orders",
                FoodTypeEnum.Lunch => "Lunch Orders",
                FoodTypeEnum.Snacks => "Snacks Orders",
                _ => "Orders",
            };

            OrderByEmployViewDataModel model = new OrderByEmployViewDataModel
            {
                screenTitle = screenTile,
                FoodOrders = foodlist,
                FoodType = (int)foodType
            };

            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> SearchOrders(string query)
        {
            var results = await foodListingService.SearchOrdersByEmployee(query);
            return View("OrderByEmployIdx", results);
        }
    }
}
