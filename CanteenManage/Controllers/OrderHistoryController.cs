using CanteenManage.Models;
using CanteenManage.Repo.Contexts;
using CanteenManage.Repo.Models;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        public OrderHistoryController(CanteenManageDBContext canteenManageContext)
        {
            this.canteenManageContext = canteenManageContext;
        }
        public async Task<IActionResult> Index()
        {
            if (SessionDataHelper.getSessionUserId(HttpContext.Session) is null)
            {
                return RedirectToAction("Login", "Index");
            }
            OrderHistoryPageDataModel myOrderViewDataModel = new OrderHistoryPageDataModel();
            try
            {

                myOrderViewDataModel.SnaksFoodOrders = await getOrderList(3,
                    SessionDataHelper.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.LunchFoodOrders = await getOrderList(2,
                    SessionDataHelper.getSessionUserId(HttpContext.Session)
                    );
                ////////////////////////////////////////////////////////////

                myOrderViewDataModel.BreakFastFoodOrders = await getOrderList(1,
                    SessionDataHelper.getSessionUserId(HttpContext.Session)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(myOrderViewDataModel);
        }
        [HttpPost]
        public async Task<IActionResult> addReview(IFormCollection formcollect)
        {

            try
            {
                var options = formcollect["options"];
                var review = formcollect["review_text"];
                var orderId = formcollect["order_id"];
                //!string.IsNullOrEmpty(review) &&
                if (!string.IsNullOrEmpty(options) && !string.IsNullOrEmpty(orderId))
                {
                    var foodOrder = await canteenManageContext.FoodOrders
                        .Where(fo => fo.Id == Convert.ToInt32(orderId))
                        .Include(fo => fo.Food)
                        .FirstOrDefaultAsync();
                    var foodReviewDetails = await canteenManageContext.FoodReviewDetails
                        .Where(fo => fo.FoodId == foodOrder.FoodId)
                        .FirstOrDefaultAsync();
                    if (foodReviewDetails == null)
                    {
                        foodReviewDetails = new FoodReviewDetails()
                        {
                            FoodId = foodOrder.FoodId,
                            TotalRating = Convert.ToInt32(options),
                            TotalUserCount = 1
                        };
                        canteenManageContext.FoodReviewDetails.Add(foodReviewDetails);
                    }
                    else
                    {
                        foodReviewDetails.TotalRating += Convert.ToInt32(options);
                        foodReviewDetails.TotalUserCount += 1;
                        canteenManageContext.FoodReviewDetails.Update(foodReviewDetails);
                    }
                    foodOrder.Rating = Convert.ToInt32(options);
                    foodOrder.Review = "..";
                    foodOrder.RatingCreatedAt = DateTime.Now;
                    foodOrder.Food.Rating = (foodReviewDetails.TotalRating / foodReviewDetails.TotalUserCount);
                    canteenManageContext.FoodOrders.Update(foodOrder);
                    await canteenManageContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }

        private async Task<List<FoodOrder>> getOrderList(int foodTypeId, int? employeId)
        {
            var orderList = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .Where(
                    fo => fo.Food.FoodTypeId == foodTypeId
                    && fo.EmployeId == employeId
                    && fo.OrderDate.Date > DateTime.Now.AddDays(-30).Date && fo.OrderDate.Date < DateTime.Now.Date
                    //&& daysOfWeek_for_snaks.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .ToListAsync();
            return orderList;
        }
    }
}
