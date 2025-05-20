using System.Threading;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Services
{
    public class CartService
    {
        private readonly CanteenManageDBContext Context;
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        public CartService(CanteenManageDBContext canteenManageContext, FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.Context = canteenManageContext;
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }

        //public async Task<int?> GetCartItemCount(int userId, CancellationToken cancellationToken)
        //{
        //    var foodOrderByUsercount = await DbContext.EmployeeCarts
        //        .Where(fo => fo.EmployeeId == userId)
        //        .SumAsync(fo => fo.Quantity, cancellationToken);
        //    return foodOrderByUsercount;
        //}


        public async Task<IResult> AddToCart(
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
                var foodid = int.Parse(selectedFoodId);
                var user_Id = userid ?? 0;
                var existingFoodOrder = await Context.EmployeeCarts
                    .Include(fo => fo.Food)
                    .Where(fo => fo.FoodId == foodid)
                    .Where(fo => fo.EmployeeId == user_Id)
                    .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                    .FirstOrDefaultAsync(cancellationToken);
                var food_quantity = 0;
                if (existingFoodOrder != null)
                {
                    food_quantity = existingFoodOrder.Quantity;
                    existingFoodOrder.Quantity = existingFoodOrder.Quantity + 1;
                    // Price is not calculating bcz it may changes in Food table after added to cart
                    //so we are calculating price at fetching list from cart table, what ever price is in Food table
                    //existingFoodOrder.TotalPrice = existingFoodOrder.Quantity * existingFoodOrder.Food.Price;
                    Context.EmployeeCarts.Update(existingFoodOrder);
                    food_quantity = existingFoodOrder.Quantity;
                }
                else if (existingFoodOrder == null)
                {

                    TimeSpan timeSpan = utilityServices.GetSpecificTimeSpan(foodTypeEnum);
                    EmployeeCart foodOrder = new EmployeeCart();
                    foodOrder.FoodId = foodid;
                    foodOrder.EmployeeId = user_Id;
                    foodOrder.Quantity = 1;
                    foodOrder.OrderDate = userSelected_DateTime.Date + timeSpan;
                    //
                    //var foodprice = await Context.Foods
                    //        .Where(f => f.Id == foodid)
                    //        .Select(f => f.Price)
                    //        .FirstOrDefaultAsync(cancellationToken);
                    //foodOrder.TotalPrice = foodOrder.Quantity * foodprice;
                    Context.EmployeeCarts.Add(foodOrder);
                    food_quantity = foodOrder.Quantity;

                }
                await Context.SaveChangesAsync();
                var totalFoodOrderByuser = await foodListingService.GetCartFoodQuantityOrderByUserCount(user_Id, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);

                var cart_count = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                if (food_quantity >= 5)
                {
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = food_quantity,
                        total_quantity = totalFoodOrderByuser,
                        total_quantity_cart = cart_count ?? 0,
                        message = "Can't add more than 5 times."
                    });
                }
                else
                {
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = food_quantity,
                        total_quantity = totalFoodOrderByuser,
                        total_quantity_cart = cart_count ?? 0,
                        message = ""
                    });
                }
            }
            else
            {
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    error = "User not found",
                });
            }
        }

        public async Task<IResult> RemoveFromCart(

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

            //int? userid = sessionData.UserId;
            if (sessionData.UserId != null)
            {
                var userid = sessionData.UserId ?? 0;
                var existingFoodOrder = await Context.EmployeeCarts
                .Include(fo => fo.Food)
                .Where(fo => fo.FoodId == int.Parse(selectedFoodId))
                .Where(fo => fo.EmployeeId == userid)
                .FirstOrDefaultAsync(cancellationToken);
                var food_quantity = 0;
                int foodType = -1;
                if (existingFoodOrder != null)
                {
                    foodType = existingFoodOrder.Food.FoodTypeId;
                    if (existingFoodOrder.Quantity == 1)
                    {
                        Context.EmployeeCarts.Remove(existingFoodOrder);
                    }
                    else
                    {
                        existingFoodOrder.Quantity = existingFoodOrder.Quantity - 1;
                        Context.EmployeeCarts.Update(existingFoodOrder);
                        food_quantity = existingFoodOrder.Quantity;
                    }
                    Context.SaveChanges();
                }
                var totalFoodOrderByuser = foodType != -1 ?
                    await foodListingService.GetCartFoodQuantityOrderByUserCount(userid, foodType, userSelected_DateTime, cancellationToken)
                    : 0;

                var cart_count = await foodListingService.GetCartItemCount(userid, cancellationToken);
                return Results.Ok(new FoodOrderApiReturnMessage()
                {
                    food_quantity = food_quantity,
                    total_quantity = totalFoodOrderByuser,
                    total_quantity_cart = cart_count ?? 0,
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

        public async Task<List<EmployeeCart>> getCartList(int foodTypeId, int? employeId, CancellationToken cancellationToken)
        {
            var orderList = await Context.EmployeeCarts
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(
                    fo => fo.Food.FoodTypeId == foodTypeId
                    && fo.EmployeeId == employeId
                    && fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder
                    )
                    .ToListAsync(cancellationToken);
            foreach (var order in orderList)
            {
                order.TotalPrice = order.Quantity * order.Food.EmployeePrice;
            }
            return orderList;
        }
        public async Task<List<EmployeeCart>> getCartOutDateList(int? employeId, CancellationToken cancellationToken)
        {
            var orderList = await Context.EmployeeCarts
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(
                    fo => fo.EmployeeId == employeId
                    && fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
            return orderList;
        }
        //dummy return
        public async Task<int> CheckOutOfOrderInCart(FoodTypeEnum foodTypeEnum, SessionDataModel sessionData, CancellationToken cancellationToken)
        {
            List<EmployeeCart>? foodOrderByUseridlist = new List<EmployeeCart>();
            foodOrderByUseridlist = await getcartfoodlistAsync(FoodTypeEnum.Breakfast, sessionData,
                   CustomDataConstants.BreakfastTimeHour, cancellationToken);
            if (foodOrderByUseridlist != null)
            {

                foreach (var foodOrder in foodOrderByUseridlist)
                {
                    foodOrder.OutDateStatus = 1;
                }
                Context.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }
            foodOrderByUseridlist = await getcartfoodlistAsync(FoodTypeEnum.Lunch, sessionData,
                   CustomDataConstants.LunchTimeHour, cancellationToken);
            if (foodOrderByUseridlist != null)
            {

                foreach (var foodOrder in foodOrderByUseridlist)
                {
                    foodOrder.OutDateStatus = 1;
                }
                Context.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }
            foodOrderByUseridlist = await getcartfoodlistAsync(FoodTypeEnum.Snacks, sessionData,
                   CustomDataConstants.SnacksTimeHour, cancellationToken);
            if (foodOrderByUseridlist != null)
            {

                foreach (var foodOrder in foodOrderByUseridlist)
                {
                    foodOrder.OutDateStatus = 1;
                }
                Context.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }

            await Context.SaveChangesAsync();
            return 1;
            //return foodOrderByUser;
        }

        private async Task<List<EmployeeCart>?> getcartfoodlistAsync(FoodTypeEnum foodTypeEnum, SessionDataModel sessionData, int houreValue, CancellationToken cancellationToken)
        {
            var foodOrderByUseridlist = await Context.EmployeeCarts
                    .Include(f => f.Food)
                    .Where(fo => fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    )
                    .Where(fo =>
                    (fo.OrderDate.Date.Day < DateTime.Now.Date.Day) || (fo.OrderDate.Date.Day == DateTime.Now.Day && fo.OrderDate.Hour >= houreValue)
                    )
                    .Where(fo => fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder)

                    .ToListAsync(cancellationToken);
            return foodOrderByUseridlist;
        }

        public async Task<IResult> ClearCart(SessionDataModel sessionData, int foodId, CancellationToken cancellationToken, int? foodTypeEnum = null)
        {
            List<EmployeeCart>? foodOrderByUseridlist = new List<EmployeeCart>();
            if (foodId == -1)
            {
                if (foodTypeEnum == (int)FoodTypeEnum.Breakfast)
                {
                    foodOrderByUseridlist = await Context.EmployeeCarts
                    .Where(fo =>
                    fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    //fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
                }
                else if (foodTypeEnum == (int)FoodTypeEnum.Lunch)
                {
                    foodOrderByUseridlist = await Context.EmployeeCarts
                    .Where(fo =>
                    fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    //fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
                }
                else if (foodTypeEnum == (int)FoodTypeEnum.Snacks)
                {
                    foodOrderByUseridlist = await Context.EmployeeCarts
                    .Where(fo =>
                    fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    //fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
                }


            }
            else
            {
                foodOrderByUseridlist = await Context.EmployeeCarts
                    //.Include(f => f.Food)
                    .Where(fo => fo.Id == foodId)
                    .Where(fo => fo.EmployeeId == sessionData.UserId
                    )
                    .ToListAsync(cancellationToken);
            }

            if (foodOrderByUseridlist.Count() > 0)
            {
                Context.EmployeeCarts.RemoveRange(foodOrderByUseridlist);
                await Context.SaveChangesAsync();
            }
            return Results.Ok(new { });
        }

        public async Task PlaceOrder(SessionDataModel sessionData, CancellationToken cancellationToken)
        {
            List<EmployeeCart> employeeCarts = await Context.EmployeeCarts
                .Include(f => f.Food)
                .Where(f => f.EmployeeId == sessionData.UserId)

                .ToListAsync(cancellationToken);
            ///////////add breakfast orders
            ///
            TimeSpan ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Breakfast);
            var breakfastFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Breakfast && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
            foreach (EmployeeCart item in breakfastFoodItems)
            {


                FoodOrder foodOrder = new FoodOrder();
                foodOrder.FoodId = item.Food.Id;
                foodOrder.FoodName = item.Food.Name;
                foodOrder.EmployeeId = sessionData.UserIdOrZero;
                foodOrder.OrderDate = item.OrderDate.Date + ts;
                foodOrder.OrderUpdateDate = DateTime.Now;
                foodOrder.Quantity = item.Quantity;
                foodOrder.OrderCompleteStatus = (int)OrderCompleteStatusEnum.Pending;
                foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                Context.FoodOrders.Add(foodOrder);
            }
            Context.EmployeeCarts.RemoveRange(breakfastFoodItems);
            ////////////////////lunch orders
            ///
            ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Lunch);
            var LunchFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Lunch && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
            foreach (EmployeeCart item in LunchFoodItems)
            {


                FoodOrder foodOrder = new FoodOrder();
                foodOrder.FoodId = item.Food.Id;
                foodOrder.FoodName = item.Food.Name;
                foodOrder.EmployeeId = sessionData.UserIdOrZero;
                foodOrder.OrderDate = item.OrderDate.Date + ts;
                foodOrder.OrderUpdateDate = DateTime.Now;
                foodOrder.Quantity = item.Quantity;

                foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                Context.FoodOrders.Add(foodOrder);
            }
            Context.EmployeeCarts.RemoveRange(LunchFoodItems);
            ////// snacks orders
            ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Snacks);
            var SnacksFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Snacks && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
            foreach (EmployeeCart item in SnacksFoodItems)
            {


                FoodOrder foodOrder = new FoodOrder();
                foodOrder.FoodId = item.Food.Id;
                foodOrder.FoodName = item.Food.Name;
                foodOrder.EmployeeId = sessionData.UserIdOrZero;
                foodOrder.OrderDate = item.OrderDate.Date + ts;
                foodOrder.OrderUpdateDate = DateTime.Now;
                foodOrder.Quantity = item.Quantity;

                foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                Context.FoodOrders.Add(foodOrder);
            }
            Context.EmployeeCarts.RemoveRange(SnacksFoodItems);

            Context.EmployeeCarts.RemoveRange(employeeCarts);

            await Context.SaveChangesAsync();
        }



    }
}
