using System.Collections.Generic;
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
        public OrdersByEmployController(FoodListingService foodListingService)
        {
            this.foodListingService = foodListingService;
            //this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> OrderByEmployIdx(string FoodType, CancellationToken cancellationToken, string searchTerm = "")
        {
            FoodTypeEnum foodType = FoodTypeEnum.Breakfast;
            List<EmployeeFoodOrdersTableDataModel> foodOrders = new List<EmployeeFoodOrdersTableDataModel>();

            try
            {
                if (!string.IsNullOrEmpty(FoodType))
                {
                    var foodtype_qp = Convert.ToInt32(FoodType);
                    if (foodtype_qp < 1)
                    {
                        searchTerm = searchTerm.Trim().ToLower();

                        foodOrders = await foodListingService.GetFoodOrdersOld(cancellationToken, searchTerm);
                    }
                    else
                    {
                        foodType = (FoodTypeEnum)foodtype_qp;
                        searchTerm = searchTerm.Trim().ToLower();

                        foodOrders = await foodListingService.GetFoodOrdersToday(foodType, cancellationToken, searchTerm);
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log error
            }

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
                FoodOrders = foodOrders,
                FoodType = (int)foodType
            };

            return View(model);
        }

        //public async Task<IActionResult> OrderByEmployBreakfast()
        //{
        //    return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "1" });
        //}
        //public async Task<IActionResult> OrderByEmployLunch()
        //{
        //    return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "2" });
        //}
        //public async Task<IActionResult> OrderByEmploySnacks()
        //{
        //    return this.RedirectToAction("OrderByEmployIdx", new { FoodType = "3" });
        //}
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
