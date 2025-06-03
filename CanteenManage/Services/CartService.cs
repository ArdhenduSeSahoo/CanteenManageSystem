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

                if (userSelected_DateTime_null?.Date < DateTime.Now.Date)
                {
                    var cart_counts = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                    return Results.Ok(new FoodOrderApiReturnMessage()
                    {
                        food_quantity = 0,
                        total_quantity = 0,
                        total_quantity_cart = cart_counts ?? 0,
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
                    if (cannotplaceorder)
                    {
                        var cart_counts = await foodListingService.GetCartItemCount(user_Id, cancellationToken);
                        return Results.Ok(new FoodOrderApiReturnMessage()
                        {
                            food_quantity = 0,
                            total_quantity = 0,
                            total_quantity_cart = cart_counts ?? 0,
                            message = errormessage,
                        });
                    }
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
                    existingFoodOrder.Quantity = existingFoodOrder.Quantity + 1;
                    // Price is not calculating bcz it may changes in Food table after added to cart
                    //so we are calculating price at fetching list from cart table, what ever price is in Food table
                    //existingFoodOrder.TotalPrice = existingFoodOrder.Quantity * existingFoodOrder.Food.Price;
                    contextDB.EmployeeCarts.Update(existingFoodOrder);
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
                    contextDB.EmployeeCarts.Add(foodOrder);
                    food_quantity = foodOrder.Quantity;

                }
                await contextDB.SaveChangesAsync();
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
        public async Task<List<CartItemInOrder>> getCartItemInOrderList(int? employeId, CancellationToken cancellationToken)
        {

            var existingcartOrder = await contextDB.EmployeeCarts
                .GroupJoin(
                    contextDB.FoodOrders,
                    cart => cart.FoodId,
                    order => order.FoodId,
                    (cart, order) => new { cart, order }
                )
                .Where(x => x.cart.EmployeeId == employeId
                && x.order.Any(ord => ord.EmployeeId == employeId
                    && ord.IsCanceled == false
                    && ord.IsCompleted == false
                    && x.cart.OrderDate.Date == ord.OrderDateCustom.Date
                    )
                )
                .Select(x => new CartItemInOrder
                {
                    ItemName = x.cart.Food.Name,
                    OrderDate = x.cart.OrderDate,
                })
                .ToListAsync(cancellationToken);
            //var existintorder = contextDB.EmployeeCarts.LeftJoin(
            //        contextDB.FoodOrders,
            //        cart => cart.FoodId,
            //        order => order.FoodId,
            //        (cart, order) => new { cart, order }
            //    )
            //    .Where(x => x.order.EmployeeId == employeId
            //    && x.order.IsCanceled == false
            //    && x.order.IsCompleted == false
            //    && x.cart.OrderDate.Date == x.order.OrderDateCustom.Date
            //    )
            //    .Select(x => new CartItemInOrder
            //    {
            //        ItemName = x.cart.Food.Name,
            //        OrderDate = x.cart.OrderDate,
            //    })
            //    .ToList();
            return existingcartOrder;
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
                    .Where(fo =>
                    (fo.OrderDate.Date < DateTime.Now.Date) || (fo.OrderDate.Date == DateTime.Now && fo.OrderDate.Hour >= houreValue)
                    )
                    .Where(fo => fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder)

                    .ToListAsync(cancellationToken);
            return foodOrderByUseridlist;
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
                    OrderID_inc++;
                    FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);
                    //FoodOrder foodOrder = new FoodOrder();
                    //foodOrder.FoodId = item.Food.Id;
                    //foodOrder.FoodName = item.Food.Name;
                    //foodOrder.EmployeeId = sessionData.UserIdOrZero;
                    //foodOrder.OrderDate = item.OrderDate.Date + ts;
                    //foodOrder.OrderUpdateDate = DateTime.Now;
                    //foodOrder.Quantity = item.Quantity;
                    //foodOrder.OrderCompleteStatus = (int)OrderCompleteStatusEnum.Pending;
                    //foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                    //foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                    //foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                    contextDB.FoodOrders.Add(foodOrder);

                }
                contextDB.EmployeeCarts.RemoveRange(breakfastFoodItems);
                ////////////////////lunch orders
                ///
                ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Lunch);
                var LunchFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Lunch && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in LunchFoodItems)
                {
                    OrderID_inc++;
                    FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);
                    //FoodOrder foodOrder = new FoodOrder();
                    //foodOrder.FoodId = item.Food.Id;
                    //foodOrder.FoodName = item.Food.Name;
                    //foodOrder.EmployeeId = sessionData.UserIdOrZero;
                    //foodOrder.OrderDate = item.OrderDate.Date + ts;
                    //foodOrder.OrderUpdateDate = DateTime.Now;
                    //foodOrder.Quantity = item.Quantity;

                    //foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                    //foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                    //foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                    contextDB.FoodOrders.Add(foodOrder);
                    //OrderID_inc++;
                }
                contextDB.EmployeeCarts.RemoveRange(LunchFoodItems);
                ////// snacks orders
                ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Snacks);
                var SnacksFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Snacks && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
                foreach (EmployeeCart item in SnacksFoodItems)
                {
                    OrderID_inc++;
                    FoodOrder foodOrder = GetFoodOrderObj(item, sessionData.UserIdOrZero, ts, orderPlacedID, OrderID_inc);
                    //FoodOrder foodOrder = new FoodOrder();
                    //foodOrder.FoodId = item.Food.Id;
                    //foodOrder.FoodName = item.Food.Name;
                    //foodOrder.EmployeeId = sessionData.UserIdOrZero;
                    //foodOrder.OrderDate = item.OrderDate.Date + ts;
                    //foodOrder.OrderUpdateDate = DateTime.Now;
                    //foodOrder.Quantity = item.Quantity;

                    //foodOrder.TotalPrice = foodOrder.Quantity * item.Food.Price;
                    //foodOrder.TotalEmployeePrice = foodOrder.Quantity * item.Food.EmployeePrice;
                    //foodOrder.TotalSubsidyPrice = foodOrder.Quantity * item.Food.SubsidyPrice;

                    contextDB.FoodOrders.Add(foodOrder);
                    //OrderID_inc++;
                }
                contextDB.EmployeeCarts.RemoveRange(SnacksFoodItems);
            }
            contextDB.EmployeeCarts.RemoveRange(employeeCarts);

            contextDB.SaveChangesAsync();
        }

        private FoodOrder GetFoodOrderObj(EmployeeCart item, int UserID, TimeSpan ts, string OrderPlacedID, int orderID_int)
        {
            //var last_serialNo = contextDB.FoodOrders.Select(fo => fo.OrderSerialNumber).DefaultIfEmpty().Max();
            //var datetimenow = DateTime.Now;
            //if ((last_serialNo) == 0)
            //{
            //    last_serialNo = 1000000;
            //}
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

        //public async Task PlaceOrderNew(SessionDataModel sessionData, CancellationToken cancellationToken)
        //{
        //    List<EmployeeCart> employeeCarts = await contextDB.EmployeeCarts
        //        .Include(f => f.Food)
        //        .Where(f => f.EmployeeId == sessionData.UserId)

        //        .ToListAsync(cancellationToken);
        //    if (sessionData.UserIdOrZero == 0 || sessionData.UserEmpIdOrNull == null)
        //    {
        //        return;
        //    }
        //    if (employeeCarts.Count <= 0)
        //    {
        //        return;
        //    }
        //    var getOrdersNotCanceled = employeeCarts.Where(f => f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder).ToList();
        //    if (getOrdersNotCanceled.Count > 0)
        //    {

        //        ///////////get food order object
        //        FoodOrder foodOrder = await GetFoodOrderObjAsync(contextDB, cancellationToken, sessionData.UserIdOrZero);
        //        ///////////add breakfast orders
        //        ///
        //        TimeSpan ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Breakfast);
        //        var breakfastFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Breakfast && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
        //        List<FoodOrderFoodDetail> foodOrderFoodDetails_breakfast = new List<FoodOrderFoodDetail>();
        //        foreach (EmployeeCart item in breakfastFoodItems)
        //        {
        //            var foodOrderFoodDetail = GetFoodOrderFoodDetail(foodOrder, item, ts, sessionData.UserIdOrZero, sessionData.UserEmpIdOrNull, cancellationToken);
        //            foodOrderFoodDetails_breakfast.Add(foodOrderFoodDetail);

        //        }
        //        foreach (var foodOrderFoodDetail in foodOrderFoodDetails_breakfast)
        //        {
        //            foodOrder.FoodOrderFoodDetails?.Add(foodOrderFoodDetail);
        //        }
        //        //Context.EmployeeCarts.RemoveRange(breakfastFoodItems);
        //        ////////////////////lunch orders
        //        ///
        //        ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Lunch);
        //        var LunchFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Lunch && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
        //        List<FoodOrderFoodDetail> foodOrderFoodDetails_lunch = new List<FoodOrderFoodDetail>();
        //        foreach (EmployeeCart item in LunchFoodItems)
        //        {
        //            var foodOrderFoodDetail = GetFoodOrderFoodDetail(foodOrder, item, ts, sessionData.UserIdOrZero, sessionData.UserEmpIdOrNull, cancellationToken);
        //            foodOrderFoodDetails_lunch.Add(foodOrderFoodDetail);

        //        }
        //        foreach (var foodOrderFoodDetail in foodOrderFoodDetails_lunch)
        //        {
        //            foodOrder.FoodOrderFoodDetails?.Add(foodOrderFoodDetail);
        //        }

        //        //Context.EmployeeCarts.RemoveRange(LunchFoodItems);
        //        ////// snacks orders
        //        ts = utilityServices.GetSpecificTimeSpan(FoodTypeEnum.Snacks);
        //        var SnacksFoodItems = employeeCarts.Where(f => f.Food.FoodTypeId == (int)FoodTypeEnum.Snacks && f.OutDateStatus == (int)CartFoodOutDateEnum.InOrder);
        //        List<FoodOrderFoodDetail> foodOrderFoodDetails_snacks = new List<FoodOrderFoodDetail>();
        //        foreach (EmployeeCart item in SnacksFoodItems)
        //        {
        //            var foodOrderFoodDetail = GetFoodOrderFoodDetail(foodOrder, item, ts, sessionData.UserIdOrZero, sessionData.UserEmpIdOrNull, cancellationToken);
        //            foodOrderFoodDetails_snacks.Add(foodOrderFoodDetail);

        //        }
        //        //Context.EmployeeCarts.RemoveRange(SnacksFoodItems);
        //        foreach (var foodOrderFoodDetail in foodOrderFoodDetails_snacks)
        //        {
        //            foodOrder.FoodOrderFoodDetails?.Add(foodOrderFoodDetail);
        //        }


        //        var totalPrice = foodOrder.FoodOrderFoodDetails?.Sum(f => f.TotalPrice);
        //        var totalEmployeePrice = foodOrder.FoodOrderFoodDetails?.Sum(f => f.TotalEmployeePrice);
        //        var totalSubsidyPrice = foodOrder.FoodOrderFoodDetails?.Sum(f => f.TotalSubsidyPrice);
        //        var totalQuantity = foodOrder.FoodOrderFoodDetails?.Sum(_ => _.Quantity);
        //        foodOrder.TotalPrice = totalPrice ?? 0;
        //        foodOrder.TotalEmployeePrice = totalEmployeePrice ?? 0;
        //        foodOrder.TotalSubsidyPrice = totalSubsidyPrice ?? 0;
        //        foodOrder.Quantity = totalQuantity ?? 0;


        //        if (foodOrder.FoodOrderFoodDetails?.Count > 0)
        //        {
        //            contextDB.FoodOrders.Add(foodOrder);
        //        }

        //    }
        //    contextDB.EmployeeCarts.RemoveRange(employeeCarts);

        //    await contextDB.SaveChangesAsync();
        //}

        //private async Task<FoodOrder> GetFoodOrderObjAsync(CanteenManageDBContext canteenManageDB, CancellationToken cancellationToken, int userID)
        //{
        //    var last_serialNo = await canteenManageDB.FoodOrders.Select(fo => fo.OrderSerialNumber).DefaultIfEmpty().MaxAsync();
        //    var datetimenow = DateTime.Now;
        //    var orderid = "ORD" + datetimenow.ToString("ddMMyy") + (last_serialNo + 1).ToString();
        //    FoodOrder foodOrder = new FoodOrder()
        //    {
        //        OrderID = orderid,
        //        OrderPlacedID = "",
        //        FoodId = 32,
        //        EmployeeId = userID,
        //        OrderDate = DateTime.Now,
        //        OrderUpdateDate = DateTime.Now,
        //        //Quantity = item.Quantity,
        //        //OrderCompleteStatus = (int)OrderCompleteStatusEnum.Pending,
        //        //TotalPrice = item.Quantity * item.Food.Price,
        //        //TotalEmployeePrice = item.Quantity * item.Food.EmployeePrice,
        //        //TotalSubsidyPrice = item.Quantity * item.Food.SubsidyPrice,
        //        OrderSerialNumber = last_serialNo + 1,
        //        IsCanceled = false,
        //        IsCompleted = false,
        //    };
        //    return foodOrder;
        //}

        //private FoodOrderFoodDetail GetFoodOrderFoodDetail(FoodOrder foodOrder, EmployeeCart CartItem, TimeSpan ts, int employId, string employEid, CancellationToken cancellationToken)
        //{
        //    FoodOrderFoodDetail foodOrderFoodDetail = new FoodOrderFoodDetail()
        //    {
        //        FoodOrder = foodOrder,
        //        FoodOrder_OrderID = foodOrder.OrderID,
        //        FoodId = CartItem.Food.Id,
        //        FoodName = CartItem.Food.Name,
        //        FoodTypeId = CartItem.Food.FoodTypeId,
        //        EmployeeId = employId,
        //        EmployeeEId = employEid,
        //        Quantity = CartItem.Quantity,
        //        TotalPrice = CartItem.Quantity * CartItem.Food.Price,
        //        TotalEmployeePrice = CartItem.Quantity * CartItem.Food.EmployeePrice,
        //        TotalSubsidyPrice = CartItem.Quantity * CartItem.Food.SubsidyPrice,
        //        IsCompleted = false,
        //        CompletedAt = null,
        //        IsCanceled = false,
        //        CanceledAt = null,
        //        OrderDateCustom = CartItem.OrderDate.Date + ts,
        //        OrderDate = DateTime.Now,
        //        OrderUpdateDate = DateTime.Now
        //    };
        //    return foodOrderFoodDetail;

        //}

    }
}
