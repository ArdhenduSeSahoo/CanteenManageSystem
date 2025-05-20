using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;

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


        public async Task<IResult> AddFoodOrder(FoodTypeEnum foodTypeEnum,
            FoodOrdersFormBodyModel foodOrdersFormBodyModel,
            SessionDataModel sessionData,
            CancellationToken cancellationToken)
        {

            var selectedFoodId = foodOrdersFormBodyModel.FoodOrderId;
            DateTime? userSelected_DateTime_null = sessionData.UserSelectedDate;
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
            if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
            {
                return Results.Ok(new { });
            }

            int? userid = sessionData.UserId;
            if (userid != null)
            {
                var foodid = int.Parse(selectedFoodId);
                var existingFoodOrder = await canteenManageContext.FoodOrders
                    .Include(fo => fo.Food)
                    .Where(fo => fo.FoodId == foodid)
                    .Where(fo => fo.EmployeeId == userid)
                    .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                    .FirstOrDefaultAsync(cancellationToken);

                var food_quantity = 0;

                if (existingFoodOrder != null)
                {
                    food_quantity = existingFoodOrder.Quantity;
                }
                if (existingFoodOrder != null && existingFoodOrder?.Quantity < 5)
                {
                    existingFoodOrder.Quantity = existingFoodOrder.Quantity + 1;
                    existingFoodOrder.TotalPrice = existingFoodOrder.Quantity * existingFoodOrder.Food.Price;
                    existingFoodOrder.OrderUpdateDate = DateTime.Now;
                    canteenManageContext.FoodOrders.Update(existingFoodOrder);
                    food_quantity = existingFoodOrder.Quantity;
                }
                else if (existingFoodOrder == null)
                {

                    TimeSpan ts = utilityServices.GetSpecificTimeSpan(foodTypeEnum);

                    FoodOrder foodOrder = new FoodOrder();
                    foodOrder.FoodId = foodid;
                    foodOrder.EmployeeId = userid ?? 0;
                    foodOrder.OrderDate = userSelected_DateTime.Date + ts;
                    foodOrder.OrderUpdateDate = DateTime.Now;
                    foodOrder.Quantity = 1;

                    var foodprice = await canteenManageContext.Foods
                            .Where(f => f.Id == userid)
                            .Select(f => f.Price)
                            .FirstOrDefaultAsync(cancellationToken);
                    foodOrder.TotalPrice = foodOrder.Quantity * foodprice;
                    canteenManageContext.FoodOrders.Add(foodOrder);
                    food_quantity = foodOrder.Quantity;
                }
                canteenManageContext.SaveChanges();

                var totalFoodOrderByuser = await foodListingService.GetFoodOrdersByUserId(userid ?? 0, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);
                var totalFoodOrderQuantity = totalFoodOrderByuser.Sum(fo => fo.Quantity);
                if (food_quantity >= 5)
                {
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = food_quantity,
                        total_quantity = totalFoodOrderQuantity,
                        message = "Can't add more than 5 times."
                    });
                }
                else
                {
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = food_quantity,
                        total_quantity = totalFoodOrderQuantity,
                        message = ""
                    });
                }
                //return Tuple.Create(food_quantity, totalFoodOrderByuser.Sum(fo => fo.Quantity));
            }
            else
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "User not found",
                });
            }
        }


        public async Task<IResult> RemoveFoodOrder(
            FoodTypeEnum foodTypeEnum,
            FoodOrdersFormBodyModel foodOrdersFormBodyModel,
            SessionDataModel sessionData,
            CancellationToken cancellationToken
            )
        {
            var selectedFoodId = foodOrdersFormBodyModel.FoodOrderId;
            DateTime? userSelected_DateTime_null = sessionData.UserSelectedDate;
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
            if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
            {
                return Results.Ok(new { });
            }

            int? userid = sessionData.UserId;
            if (userid != null)
            {
                var existingFoodOrder = await canteenManageContext.FoodOrders
                .Include(fo => fo.Food)
                .Where(fo => fo.FoodId == int.Parse(selectedFoodId))
                .Where(fo => fo.EmployeeId == userid)
                .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                .FirstOrDefaultAsync(cancellationToken);

                var food_quantity = 0;
                if (existingFoodOrder != null)
                {
                    if (existingFoodOrder.Quantity == 1)
                    {
                        canteenManageContext.FoodOrders.Remove(existingFoodOrder);
                    }
                    else
                    {
                        existingFoodOrder.Quantity = existingFoodOrder.Quantity - 1;
                        existingFoodOrder.OrderUpdateDate = userSelected_DateTime;
                        canteenManageContext.FoodOrders.Update(existingFoodOrder);
                        food_quantity = existingFoodOrder.Quantity;
                    }
                    canteenManageContext.SaveChanges();
                }

                var totalFoodOrderByuser = await foodListingService.GetFoodOrdersByUserId(userid ?? 0, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);
                var totalfoodOrderQuantity = totalFoodOrderByuser.Sum(fo => fo.Quantity);
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    food_quantity = food_quantity,
                    total_quantity = totalfoodOrderQuantity,
                    message = ""
                });
            }
            else
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "User not found",
                });
            }
        }

        public async Task<List<FoodOrder>> getOrderList(int foodTypeId, int? employeId)
        {
            var orderList = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(
                    fo => fo.Food.FoodTypeId == foodTypeId
                    && fo.EmployeeId == employeId
                    && fo.OrderDate.Date > DateTime.Now.AddDays(-30).Date && fo.OrderDate.Date < DateTime.Now.Date
                    //&& daysOfWeek_for_snaks.Select(s => s.DateTime.Date).Contains(fo.OrderDate.Date)
                    )
                    .ToListAsync();
            return orderList;
        }
    }
}
