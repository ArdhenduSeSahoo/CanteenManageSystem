using System.Threading;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CanteenManage.Services
{
    public class CartService
    {
        private readonly CanteenManageDBContext contextDB;
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        public CartService(CanteenManageDBContext canteenManageContext, FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.contextDB = canteenManageContext;
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }

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

                var existingFoodOrder_default = await contextDB.EmployeeCarts
                  .Where(fo => fo.FoodId == foodid)
                  .Where(fo => fo.EmployeeId == user_Id)
                  .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                  .FirstOrDefaultAsync(cancellationToken);
                var totalFoodOrderByuser_default = await foodListingService.GetCartFoodQuantityOrderByUserCount(user_Id, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);

                var cart_count_default = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                if (userSelected_DateTime_null?.Date < DateTime.Now.Date)
                {
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = existingFoodOrder_default?.Quantity ?? 0,
                        total_quantity = totalFoodOrderByuser_default,
                        total_quantity_cart = cart_count_default ?? 0,
                        message = "Can not order on back date.",
                    });
                }
                if (userSelected_DateTime_null?.Date == DateTime.Now.Date)
                {
                    bool cannotplaceorder = false;
                    string errormessage = "";
                    if (foodTypeEnum == FoodTypeEnum.Breakfast && userSelected_DateTime.Hour >= CustomDataConstants.BreakfastTimeHour)
                    {
                        errormessage = "Breakfast time is over.";
                        cannotplaceorder = true;
                    }
                    else if (foodTypeEnum == FoodTypeEnum.Lunch && userSelected_DateTime.Hour >= CustomDataConstants.LunchTimeHour)
                    {
                        errormessage = "Lunch time is over.";
                        cannotplaceorder = true;
                    }
                    else if (foodTypeEnum == FoodTypeEnum.Snacks && userSelected_DateTime.Hour >= CustomDataConstants.SnacksTimeHour)
                    {
                        errormessage = "Snacks time is over.";
                        cannotplaceorder = true;
                    }
                    else if (foodTypeEnum == FoodTypeEnum.Dinner && userSelected_DateTime.Hour >= CustomDataConstants.DinnerTimeHour)
                    {
                        errormessage = "Dinner time is over.";
                        cannotplaceorder = true;
                    }
                    if (cannotplaceorder)
                    {
                        return Results.Ok(new FoodOrderApiReturnMessage()
                        {
                            food_quantity = existingFoodOrder_default?.Quantity ?? 0,
                            total_quantity = totalFoodOrderByuser_default,
                            total_quantity_cart = cart_count_default ?? 0,
                            message = errormessage,
                        });
                    }
                }
                if (await ValidateFoodForSelectedDate(foodTypeEnum, foodid, sessionData, cancellationToken) == false)
                {
                    var cart_counts = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = existingFoodOrder_default?.Quantity ?? 0,
                        total_quantity = totalFoodOrderByuser_default,
                        total_quantity_cart = cart_count_default ?? 0,
                        message = "Food is not available for selected date.",
                    });
                }

                var existingFoodOrder = await contextDB.EmployeeCarts
                    .Include(fo => fo.Food)
                    .Where(fo => fo.FoodId == foodid)
                    .Where(fo => fo.EmployeeId == user_Id)
                    .Where(fo => fo.OrderDate.Date == userSelected_DateTime.Date)
                    .FirstOrDefaultAsync(cancellationToken);
                var food_quantity = 0;
                if (existingFoodOrder != null)
                {
                    food_quantity = existingFoodOrder.Quantity;
                    //if (food_quantity <= 4)
                    {
                        existingFoodOrder.Quantity = existingFoodOrder.Quantity + 1;
                        // Price is not calculating bcz it may changes in Food table after added to cart
                        //so we are calculating price at fetching list from cart table, what ever price is in Food table
                        //existingFoodOrder.TotalPrice = existingFoodOrder.Quantity * existingFoodOrder.Food.Price;
                        contextDB.EmployeeCarts.Update(existingFoodOrder);
                        food_quantity = existingFoodOrder.Quantity;
                    }
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
                    contextDB.EmployeeCarts.Add(foodOrder);
                    food_quantity = foodOrder.Quantity;

                }
                await contextDB.SaveChangesAsync();
                var totalFoodOrderByuser = await foodListingService.GetCartFoodQuantityOrderByUserCount(user_Id, (int)foodTypeEnum, userSelected_DateTime, cancellationToken);

                var cart_count = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                //if (food_quantity >= 5)
                //{
                //    return Results.Ok(new FoodOrderApiReturnMessage()
                //    {
                //        food_quantity = food_quantity,
                //        total_quantity = totalFoodOrderByuser,
                //        total_quantity_cart = cart_count ?? 0,
                //        message = "Can't add more than 5 times."
                //    });
                //}
                //else
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
                var existingFoodOrder = await contextDB.EmployeeCarts
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
                        contextDB.EmployeeCarts.Remove(existingFoodOrder);
                    }
                    else
                    {
                        existingFoodOrder.Quantity = existingFoodOrder.Quantity - 1;
                        contextDB.EmployeeCarts.Update(existingFoodOrder);
                        food_quantity = existingFoodOrder.Quantity;
                    }
                    contextDB.SaveChanges();
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

        public async Task<bool> ValidateFoodForSelectedDate(FoodTypeEnum foodType, int foodID, SessionDataModel sessionData, CancellationToken cancellationToken)
        {
            if (sessionData.UserSelectedDate == null)
            {
                return false;
            }
            var userSelected_DateTime = sessionData.UserSelectedDate.Value;

            if (userSelected_DateTime.Date < DateTime.Now.Date)
            {
                return false;
            }
            if (userSelected_DateTime.Date == DateTime.Now.Date)
            {
                if (foodType == FoodTypeEnum.Breakfast && int.Parse(DateTime.Now.ToString("HH")) >= CustomDataConstants.BreakfastTimeHourEnd)
                {
                    return false;
                }
                else if (foodType == FoodTypeEnum.Lunch && int.Parse(DateTime.Now.ToString("HH")) >= CustomDataConstants.LunchTimeHourEnd)
                {
                    return false;
                }
                else if (foodType == FoodTypeEnum.Snacks && int.Parse(DateTime.Now.ToString("HH")) >= CustomDataConstants.SnacksTimeHour)
                {
                    return false;
                }
                else if (foodType == FoodTypeEnum.Dinner && int.Parse(DateTime.Now.ToString("HH")) >= CustomDataConstants.DinnerTimeHour)
                {
                    return false;
                }
            }
            return await ValidateFoodForSelectedDate(foodType, foodID, userSelected_DateTime, cancellationToken);
        }

        public async Task<bool> ValidateFoodForSelectedDate(FoodTypeEnum foodType, int foodID, DateTime SelectedDate, CancellationToken cancellationToken)
        {
            var dayOfWeek = (int?)SelectedDate.DayOfWeek;
            var weekOfMonth = GetWeekOfMonth(SelectedDate);
            if (weekOfMonth == 5)
            {
                weekOfMonth = 1;
            }
            var food = await contextDB.Foods.Where(fo => fo.Id == foodID && fo.FoodAvailabilityDays.Any(fa =>
                (fa.DayOfWeek == dayOfWeek) &&
                (fa.WeekOfMonth == weekOfMonth)
                )).FirstOrDefaultAsync(cancellationToken);
            if (food != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetWeekOfMonth(DateTime date)
        {
            // Find the first day of the month
            DateTime firstOfMonth = new DateTime(date.Year, date.Month, 1);

            // Get the offset (how many days before the first Sunday)
            int offset = (int)firstOfMonth.DayOfWeek;

            // Calculate week number (Sunday as first day of week)
            return ((date.Day + offset - 1) / 7) + 1;
        }
        public async Task<List<EmployeeCart>> getCartList(int foodTypeId, int? employeId, CancellationToken cancellationToken)
        {
            var orderList = await contextDB.EmployeeCarts
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
            var orderList = await contextDB.EmployeeCarts
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(
                    fo => fo.EmployeeId == employeId
                    && fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
            return orderList;
        }
        public async Task<List<CartItemInOrder>> getCartItemInOrderList(int? employeeId, CancellationToken cancellationToken)
        {

            var cart_foods1 = await contextDB.EmployeeCarts
            .Include(x => x.Food)
            .Where(ec => ec.EmployeeId == employeeId)
            .Select(cf => new CartItemInOrder
            {
                ItemName = cf.Food.Name,
                OrderDate = cf.OrderDate,
                Quantity = cf.Food.FoodOrders.Where(fo => fo.IsCanceled == false
                    && fo.IsCompleted == false
                    && fo.EmployeeId == employeeId
                    && fo.FoodId == cf.FoodId
                    && fo.OrderDateCustom.Date == cf.OrderDate.Date).Sum(or => or.Quantity),

            })
            .ToListAsync(cancellationToken)
            ;
            List<CartItemInOrder> cart_items_in_order = new List<CartItemInOrder>();
            cart_items_in_order = cart_foods1.Where(cf => cf.Quantity > 0).ToList();

            return cart_items_in_order;
        }

        public async Task<List<CartItemInOrder>> getMaxCartItemInOrderList(int? employeeId, CancellationToken cancellationToken)
        {

            var cart_foods1 = await contextDB.EmployeeCarts
                .Include(x => x.Food)
                .Where(ec => ec.EmployeeId == employeeId)
                .Select(cf => new CartItemInOrder
                {
                    ItemName = cf.Food.Name,
                    OrderDate = cf.OrderDate,
                    Quantity = cf.Food.FoodOrders.Where(fo => fo.IsCanceled == false
                        && fo.IsCompleted == false
                        && fo.EmployeeId == employeeId
                        && fo.FoodId == cf.FoodId
                        && fo.OrderDateCustom.Date == cf.OrderDate.Date).Sum(or => or.Quantity) + cf.Quantity,
                })
                .ToListAsync(cancellationToken)
                ;
            List<CartItemInOrder> cart_items_in_order = new List<CartItemInOrder>();
            cart_items_in_order = cart_foods1.Where(cf => cf.Quantity > CustomDataConstants.MaxCartItemCount).ToList();

            return cart_items_in_order;
        }

        //dummy return
        public async Task<int> CheckOutOfOrderInCart(SessionDataModel sessionData, CancellationToken cancellationToken)
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
                contextDB.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }
            foodOrderByUseridlist = await getcartfoodlistAsync(FoodTypeEnum.Lunch, sessionData,
                   CustomDataConstants.LunchTimeHour, cancellationToken);
            if (foodOrderByUseridlist != null)
            {

                foreach (var foodOrder in foodOrderByUseridlist)
                {
                    foodOrder.OutDateStatus = 1;
                }
                contextDB.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }
            foodOrderByUseridlist = await getcartfoodlistAsync(FoodTypeEnum.Snacks, sessionData,
                   CustomDataConstants.SnacksTimeHour, cancellationToken);
            if (foodOrderByUseridlist != null)
            {

                foreach (var foodOrder in foodOrderByUseridlist)
                {
                    foodOrder.OutDateStatus = 1;
                }
                contextDB.EmployeeCarts.UpdateRange(foodOrderByUseridlist);
            }

            await contextDB.SaveChangesAsync();
            return 1;
            //return foodOrderByUser;
        }

        private async Task<List<EmployeeCart>?> getcartfoodlistAsync(FoodTypeEnum foodTypeEnum, SessionDataModel sessionData, int houreValue, CancellationToken cancellationToken)
        {

            var foodOrderByUseridlist = await contextDB.EmployeeCarts
                    .Include(f => f.Food)
                    .Where(fo => fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    )
                    .Where(fo => fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder)
                    .ToListAsync(cancellationToken);

            ///bcz 24 hour format can not check in sql linq we are again filtering in memory
            var foodOrderByUseridlist_24Hr = foodOrderByUseridlist.Where(fo => fo.OrderDate.Date < DateTime.Now.Date || (fo.OrderDate.Date == DateTime.Now.Date && int.Parse(DateTime.Now.ToString("HH")) >= houreValue)).ToList();
            return foodOrderByUseridlist_24Hr;
        }
        public async Task<bool> RemoveCartItem(SessionDataModel sessionData, int foodId, CancellationToken cancellationToken)
        {
            bool itemFoundandRemoved = false;
            var fooditem = await contextDB.EmployeeCarts
                .Where(ec => ec.Id == foodId && ec.EmployeeId == sessionData.UserIdOrZero)
                .FirstOrDefaultAsync(cancellationToken);
            if (fooditem != null)
            {
                itemFoundandRemoved = fooditem != null;
                contextDB.EmployeeCarts.Remove(fooditem);
                await contextDB.SaveChangesAsync();
            }
            return itemFoundandRemoved;
        }

        public async Task<IResult> ClearCart(SessionDataModel sessionData, int foodId, CancellationToken cancellationToken, int? foodTypeEnum = null)
        {
            List<EmployeeCart>? foodOrderByUseridlist = new List<EmployeeCart>();
            if (foodId == -1)
            {
                if (foodTypeEnum == (int)FoodTypeEnum.Breakfast)
                {
                    foodOrderByUseridlist = await contextDB.EmployeeCarts
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
                    foodOrderByUseridlist = await contextDB.EmployeeCarts
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
                    foodOrderByUseridlist = await contextDB.EmployeeCarts
                    .Where(fo =>
                    fo.EmployeeId == sessionData.UserId
                    &&
                    fo.Food.FoodTypeId == (int)foodTypeEnum
                    //fo.OutDateStatus == (int)CartFoodOutDateEnum.OutOfOrder
                    )
                    .ToListAsync(cancellationToken);
                }
                else if (foodTypeEnum == (int)FoodTypeEnum.Dinner)
                {
                    foodOrderByUseridlist = await contextDB.EmployeeCarts
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
                foodOrderByUseridlist = await contextDB.EmployeeCarts
                    //.Include(f => f.Food)
                    .Where(fo => fo.Id == foodId)
                    .Where(fo => fo.EmployeeId == sessionData.UserId
                    )
                    .ToListAsync(cancellationToken);
            }

            if (foodOrderByUseridlist.Count() > 0)
            {
                contextDB.EmployeeCarts.RemoveRange(foodOrderByUseridlist);
                await contextDB.SaveChangesAsync();
            }
            return Results.Ok(new { });
        }

        public void PlaceOrder(SessionDataModel sessionData, CancellationToken cancellationToken)
        {
            List<EmployeeCart> employeeCarts = contextDB.EmployeeCarts
                .Include(f => f.Food)
                .Where(f => f.EmployeeId == sessionData.UserId)

                .ToList();
            if (sessionData.UserIdOrZero == 0 || sessionData.UserEmpIdOrNull == null)
            {
                return;
            }
            if (employeeCarts.Count <= 0)
            {
                return;
            }
            var getOrdersNotCanceled = employeeCarts.Where(f => f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder).ToList();
            if (getOrdersNotCanceled.Count > 0)
            {
                var last_serialNo = contextDB.FoodOrders.Select(fo => fo.OrderSerialNumber).DefaultIfEmpty().Max();
                var datetimenow = DateTime.Now;
                if ((last_serialNo) == 0)
                {
                    last_serialNo = 1000000;
                }
                var orderPlacedID = "ORDP" + (last_serialNo + 1).ToString();
                var OrderID_inc = last_serialNo;
                ///////////add breakfast orders
                ///
                TimeSpan ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Breakfast);
                var breakfastFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Breakfast && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in breakfastFoodItems)
                {
                    if (ValidateFoodForSelectedDate(FoodTypeEnum.Breakfast, item.FoodId, item.OrderDate, cancellationToken).Result == true)
                    {

                        OrderID_inc++;
                        FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);

                        contextDB.FoodOrders.Add(foodOrder);
                    }
                }
                contextDB.EmployeeCarts.RemoveRange(breakfastFoodItems);
                ////////////////////lunch orders
                ///
                ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Lunch);
                var LunchFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Lunch && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in LunchFoodItems)
                {
                    if (ValidateFoodForSelectedDate(FoodTypeEnum.Lunch, item.FoodId, item.OrderDate, cancellationToken).Result == true)
                    {

                        OrderID_inc++;
                        FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);

                        contextDB.FoodOrders.Add(foodOrder);
                    }
                }
                contextDB.EmployeeCarts.RemoveRange(LunchFoodItems);
                ////// snacks orders
                ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Snacks);
                var SnacksFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Snacks && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in SnacksFoodItems)
                {
                    if (ValidateFoodForSelectedDate(FoodTypeEnum.Snacks, item.FoodId, item.OrderDate, cancellationToken).Result == true)
                    {
                        OrderID_inc++;
                        FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);

                        contextDB.FoodOrders.Add(foodOrder);
                    }
                }
                contextDB.EmployeeCarts.RemoveRange(SnacksFoodItems);
                ////// dinner orders
                ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Dinner);
                var DinnerFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Dinner && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in DinnerFoodItems)
                {
                    if (ValidateFoodForSelectedDate(FoodTypeEnum.Dinner, item.FoodId, item.OrderDate, cancellationToken).Result == true)
                    {
                        OrderID_inc++;
                        FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);

                        contextDB.FoodOrders.Add(foodOrder);
                    }
                }
                contextDB.EmployeeCarts.RemoveRange(DinnerFoodItems);
            }
            contextDB.EmployeeCarts.RemoveRange(employeeCarts);

            contextDB.SaveChangesAsync();
        }

        private FoodOrder GetFoodOrderObj(EmployeeCart item, int UserID, TimeSpan ts, string OrderPlacedID, int orderID_int)
        {

            var orderID = "ORD" + (orderID_int).ToString();
            FoodOrder foodOrder = new FoodOrder()
            { OrderID = orderID, OrderPlacedID = OrderPlacedID };
            foodOrder.FoodId = item.Food.Id;
            foodOrder.FoodName = item.Food.Name;
            foodOrder.EmployeeId = UserID;
            foodOrder.OrderDateCustom = item.OrderDate.Date + ts;
            foodOrder.OrderDate = DateTime.Now;
            foodOrder.OrderUpdateDate = DateTime.Now;
            foodOrder.Quantity = item.Quantity;
            foodOrder.CanceledAt = null;
            foodOrder.CompletedAt = null;
            foodOrder.IsCanceled = false;
            foodOrder.IsCompleted = false;
            foodOrder.OrderSerialNumber = orderID_int;


            foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
            foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
            foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;
            return foodOrder;
        }

    }
}
