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
        public OrderingService(CanteenManageDBContext canteenManageContext)
        {
            this.canteenManageContext = canteenManageContext;
        }

        public async Task<List<FoodOrder>> GetFoodOrdersByUserId(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await canteenManageContext.FoodOrders
               .Include(f => f.Food)
               .Where(fo => fo.EmployeeId == userId
               &&
               fo.OrderDate.Date == orderDateTime.Date
               )
               .Where(fo =>
               fo.Food.FoodTypeId == foodType
               )
               .ToListAsync(cancellationToken);
            return foodOrderByUser;
        }


        public async Task<List<Food>> GetFoodOrdersByUserId(int foodType, List<FoodOrder> foodOrdersByUser, CancellationToken cancellationToken, int foodAvailableDay = 0)
        {

            var allFoodWithUserOrderDetails = await canteenManageContext.Foods
               .Include(f => f.FoodOrders.Where(fo => foodOrdersByUser.Select(fo => fo.Id).Contains(fo.Id)))
               .Where(f => f.FoodTypeId == foodType)
               .Where(f => f.AvailableOnDay == foodAvailableDay || f.AvailableOnDay == 0)
               .ToListAsync(cancellationToken);
            return allFoodWithUserOrderDetails;
        }

        public async Task<List<CanteenFoodDetailsDTOModel>> getFoodOrderGroupList(int foodType, CancellationToken cancellationToken)
        {
            var FoodlistGrouping = await canteenManageContext.FoodOrders
                    .Include(f => f.Food)
                    .Where(f =>
                    f.Food.FoodTypeId == foodType
                    && f.OrderDate.Date >= DateTime.Now.Date
                    )
                    .GroupBy(f => new { f.FoodId, f.OrderDate.Date })
                    .Select(f => new CanteenFoodDetailsDTOModel()
                    {
                        Id = f.Max(fo => fo.Id),
                        Name = f.Max(fm => fm.Food.Name) ?? "",
                        OrderDate = f.Key.Date,
                        FoodTypeId = f.Max(fm => fm.Food.FoodTypeId),
                        Price = 0,
                        FoodQuantity = f.Sum(fo => fo.Quantity),
                        EmployId = f.Max(fo => fo.EmployeeId),
                        EmployName = f.Max(fo => fo.Employee.Name) ?? "",
                    })
                    .ToListAsync(cancellationToken);
            return FoodlistGrouping;
        }

        public async Task<IResult> AddFoodOrder(FoodTypeEnum foodTypeEnum,
            FoodOrdersFormBodyModel foodOrdersFormBodyModel,
            ISession session,
            CancellationToken cancellationToken)
        {

            var selectedFoodId = foodOrdersFormBodyModel.FoodOrderId;
            DateTime? userSelected_DateTime_null = SessionDataHelper.getDateTimeFromSession(session);
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
            if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
            {
                return Results.Ok(new { });
            }

            int? userid = SessionDataHelper.getSessionUserId(session);
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
                    TimeSpan ts = new TimeSpan();
                    if (foodTypeEnum == FoodTypeEnum.Breakfast)
                    {
                        ts = new TimeSpan(06, 05, 0);
                    }
                    else if (foodTypeEnum == FoodTypeEnum.Lunch)
                    {
                        ts = new TimeSpan(11, 05, 0);
                    }
                    else if (foodTypeEnum == FoodTypeEnum.Snacks)
                    {
                        ts = new TimeSpan(15, 05, 0);
                    }
                    else if (foodTypeEnum == FoodTypeEnum.QuickFood)
                    {
                        ts = new TimeSpan();
                    }


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

                var totalFoodOrderByuser = await GetFoodOrdersByUserId(userid ?? 0, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);
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
            ISession session,
            CancellationToken cancellationToken
            )
        {
            var selectedFoodId = foodOrdersFormBodyModel.FoodOrderId;
            DateTime? userSelected_DateTime_null = SessionDataHelper.getDateTimeFromSession(session);
            DateTime userSelected_DateTime = userSelected_DateTime_null ?? DateTime.Now;
            if (userSelected_DateTime_null == null || string.IsNullOrEmpty(selectedFoodId))
            {
                return Results.Ok(new { });
            }

            int? userid = SessionDataHelper.getSessionUserId(session);
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

                var totalFoodOrderByuser = await GetFoodOrdersByUserId(userid ?? 0, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);
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

    }
}
