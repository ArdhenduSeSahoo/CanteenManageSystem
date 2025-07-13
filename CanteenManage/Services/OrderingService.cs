using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.Wordprocessing;
using CanteenManage.Models.DTO;

namespace CanteenManage.Services
{
    public class OrderingService
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        public OrderingService(CanteenManageDBContext canteenManageContext, FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.canteenManageContext = canteenManageContext;
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }

        public async Task RemoveFoodOrder(string foodId, string foodOrderID, SessionDataModel sessionData, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(foodOrderID) && !string.IsNullOrEmpty(foodOrderID))
            {
                //var foodID = int.Parse(foodId);
                using var transaction = await canteenManageContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {


                    var foodOrder = await canteenManageContext.FoodOrders
                            .Include(fo => fo.Food)
                            .Where(fo => fo.OrderID == foodOrderID &&
                            fo.EmployeeId == sessionData.UserIdOrZero
                            //&& fo.Id == foodID
                            && !fo.IsCanceled
                            ).FirstOrDefaultAsync(cancellationToken);
                    if (foodOrder != null)
                    {

                        //if (foodOrder.Quantity > 1)
                        //{
                        //    foodOrder.Quantity -= 1;
                        //    foodOrder.TotalPrice = foodOrder.Quantity*foodOrder.Food.Price;
                        //    //foodOrder.TotalPrice = totalPrice;
                        //    foodOrder.TotalEmployeePrice = FoodDetails_Uncancled.Sum(fd => fd.TotalEmployeePrice);
                        //    foodOrder.TotalSubsidyPrice = FoodDetails_Uncancled.Sum(fd => fd.TotalSubsidyPrice);
                        //    foodOrder.Quantity = FoodDetails_Uncancled.Sum(fd => fd.Quantity);
                        //    foodOrder.OrderUpdateDate = DateTime.Now;
                        //    canteenManageContext.FoodOrders.Update(foodOrder);
                        //    await canteenManageContext.SaveChangesAsync();
                        //}
                        //else
                        {
                            //foodOrder.TotalPrice = 0;
                            //foodOrder.TotalEmployeePrice = 0;
                            //foodOrder.TotalSubsidyPrice = 0;
                            //foodOrder.Quantity = 0;
                            //foodOrder.IsCanceled = true;
                            //foodOrder.CanceledAt = DateTime.Now;
                            //canteenManageContext.FoodOrders.Update(foodOrder);
                            await canteenManageContext.FoodOrders
                                .Where(f => f.OrderID == foodOrderID
                                && f.EmployeeId == sessionData.UserIdOrZero
                                )
                                .ExecuteUpdateAsync(
                                x => x.SetProperty(f => f.IsCanceled, true)
                                      .SetProperty(f => f.CanceledAt, DateTime.Now)

                            );
                            await canteenManageContext.SaveChangesAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        //await transaction.RollbackAsync();
                        throw new Exception("Order not found");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("Values are null");
            }
        }


        public async Task<List<FoodOrderDto>> getOrderHistoryList(int foodTypeId, int? employeId)
        {
            var orderList = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(
                    fo => fo.Food.FoodTypeId == foodTypeId
                    && fo.EmployeeId == employeId
                    && fo.OrderDateCustom.Date > DateTime.Now.AddDays(-30).Date && fo.OrderDateCustom.Date < DateTime.Now.Date
                    && fo.IsCanceled == false
                    //&& daysOfWeek_for_snaks.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .OrderBy(fo => fo.OrderDateCustom)
                    .Select(fo => new FoodOrderDto()
                    {
                        OrderID = fo.OrderID,
                        FoodId = fo.FoodId,
                        FoodName = fo.FoodName,
                        OrderDate = fo.OrderDate,
                        OrderDateCustom = fo.OrderDateCustom,
                        IsCanceled = fo.IsCanceled,
                        IsCompleted = fo.IsCompleted,
                        Quantity = fo.Quantity,
                        TotalEmployeePrice = fo.TotalEmployeePrice,
                        Rating = fo.Rating,
                        Review = fo.Review,

                    })
                    .ToListAsync();
            return orderList;
        }

        public async Task<(List<FoodOrder>, int)> GetFeedbackList(CancellationToken cancellationToken, int page, int pagesize)
        {
            var feedbacklist = await canteenManageContext.FoodOrders
                .Include(f => f.Food)
                .Include(f => f.Employee)
                .AsNoTracking()
                .Where(x => x.Review != "")
            .OrderByDescending(x => x.RatingCreatedAt)
            .Skip((page - 1) * pagesize)
            .Take(pagesize)
                .ToListAsync(cancellationToken);
            var totalcount = await canteenManageContext.FoodOrders
                .AsNoTracking()
                .Where(x => x.Review != "")
                .CountAsync(cancellationToken);
            return (feedbacklist, totalcount);
        }
        //public async Task<FoodOrder> GetByIdFeedback(int FoodOrderId, string ActionTaken, CancellationToken cancellationToken)
        //{
        //    var order = await canteenManageContext.FoodOrders.FindAsync(FoodOrderId);
        //    if (order != null && !string.IsNullOrWhiteSpace(ActionTaken))
        //    {
        //        order.ActionTaken = ActionTaken;
        //        await canteenManageContext.SaveChangesAsync(cancellationToken);
        //    }
        //    return order;
        //}
        public async Task<FoodOrder?> GetByIdFeedback(int FoodOrderId, string? ActionTaken, CancellationToken cancellationToken)
        {
            if (FoodOrderId <= 0)
            {
                return null;
            }

            var order = await canteenManageContext.FoodOrders.FindAsync(FoodOrderId);

            if (order == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(ActionTaken))
            {
                order.ActionTaken = ActionTaken;
                await canteenManageContext.SaveChangesAsync(cancellationToken);
            }

            return order;
        }
        public async Task addReview(SessionDataModel sessionData, string orderID, int rating, string review)
        {
            if (sessionData.UserIdOrZero == 0)
            {
                return;
            }
            var foodOrder = await canteenManageContext.FoodOrders
                .Include(review => review.Food)
                .Where(fo => fo.OrderID == orderID && fo.EmployeeId == sessionData.UserIdOrZero)
                .FirstOrDefaultAsync();
            if (foodOrder != null)
            {
                var foodid = foodOrder?.FoodId;
                var foodReviewDetails = await canteenManageContext.FoodReviewDetails
                    .Where(fo => fo.FoodId == foodid)
                    .FirstOrDefaultAsync();
                if (foodReviewDetails == null)
                {
                    foodReviewDetails = new FoodReviewDetails()
                    {
                        FoodId = foodOrder?.FoodId,
                        TotalRating = rating,
                        TotalUserCount = 1
                    };
                    canteenManageContext.FoodReviewDetails.Add(foodReviewDetails);
                }
                else
                {
                    foodReviewDetails.TotalRating += Convert.ToInt32(rating);
                    foodReviewDetails.TotalUserCount += 1;
                    canteenManageContext.FoodReviewDetails.Update(foodReviewDetails);
                }
                foodOrder.Rating = rating;
                var substringEnd = review.Length > 100 ? 100 : review.Length;
                foodOrder.Review = string.IsNullOrWhiteSpace(review) ? "" : review.Substring(0, substringEnd);
                foodOrder.RatingCreatedAt = DateTime.Now;
                foodOrder.Food.Rating = (foodReviewDetails.TotalRating / foodReviewDetails.TotalUserCount);
                foodOrder.Food.UserRateGiven = foodReviewDetails.TotalUserCount;
                canteenManageContext.FoodOrders.Update(foodOrder);
                await canteenManageContext.SaveChangesAsync();
            }
        }
    }
}
