
using System.Security.Claims;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Models.DTO;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;
namespace CanteenManage.Services
{
    public class FoodListingService
    {
        private readonly CanteenManageDBContext contextCM;

        private readonly OrderDataCaching orderDataCaching;

        public FoodListingService(CanteenManageDBContext canteenManageContext, OrderDataCaching orderDataCaching)
        {
            this.contextCM = canteenManageContext;
            this.orderDataCaching = orderDataCaching;
        }
        public async Task<List<FoodOrder>> GetFoodOrdersByUserId(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await contextCM.FoodOrders
               .Include(f => f.Food)
               .Where(fo => fo.EmployeeId == userId
               &&
               fo.OrderDateCustom.Date == orderDateTime.Date
               )
               .Where(fo =>
               fo.Food.FoodTypeId == foodType
               )
               .ToListAsync(cancellationToken);
            return foodOrderByUser;
        }

        public async Task<List<EmployeeCart>> GetCartFoodOrdersByUser(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await contextCM.EmployeeCarts
               .Include(f => f.Food)
               .Where(fo => fo.EmployeeId == userId)
               .Where(fo =>
               fo.Food.FoodTypeId == foodType
               )
               .Where(fo =>
               fo.OrderDate.Date == orderDateTime.Date
               &&
               fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder
               )
               .ToListAsync(cancellationToken);
            return foodOrderByUser;
        }
        public async Task<int> GetCartFoodQuantityOrderByUserCount(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await contextCM.EmployeeCarts
               .Include(f => f.Food)
               .AsNoTracking()
               .Where(fo => fo.EmployeeId == userId
               &&
               fo.OrderDate.Date == orderDateTime.Date
               )
               .Where(fo =>
               fo.Food.FoodTypeId == foodType
               )
               .SumAsync(fo => fo.Quantity, cancellationToken);
            return foodOrderByUser;
        }

        public async Task<int?> GetCartItemCount(int userId, CancellationToken cancellationToken)
        {
            var foodOrderByUsercount = await contextCM.EmployeeCarts
                .Where(fo => fo.EmployeeId == userId)
                .AsNoTracking()
                .Where(fo => fo.OutDateStatus == (int)CartFoodOutDateEnum.InOrder)
                .AsNoTracking()
                .SumAsync(fo => fo.Quantity, cancellationToken);
            return foodOrderByUsercount;
        }


        public async Task<List<FoodDetails>> GetAllFoodList(FoodTypeEnum foodType, CancellationToken cancellationToken, DateTime userSelected_dateTime, SessionDataModel sessionData)
        {
            var dayOfWeek = (int?)sessionData.UserSelectedDate?.DayOfWeek;
            if (dayOfWeek == null)
            {
                throw new Exception("DayOfWeek in session cant be null");
            }
            var weekOfMonth = GetWeekOfMonth(userSelected_dateTime);

            //replacing weekOfMonth with 1 if it is 5 because week 5 and 1 are same
            if (weekOfMonth == 5)
            {
                weekOfMonth = 1;
            }
            var allFoodWithUserOrderDetails = new List<FoodDetails>();
            //if (userSelected_dateTime.Date.Day == DateTime.Now.Date.Day)
            //{
            //    if (foodType == FoodTypeEnum.Breakfast && DateTime.Now.Hour >= CustomDataConstants.BreakfastTimeHour)
            //    {
            //        return allFoodWithUserOrderDetails;
            //    }
            //    else if (foodType == FoodTypeEnum.Lunch && DateTime.Now.Hour >= CustomDataConstants.LunchTimeHour)
            //    {
            //        return allFoodWithUserOrderDetails;
            //    }
            //    else if (foodType == FoodTypeEnum.Snacks && DateTime.Now.Hour >= CustomDataConstants.SnacksTimeHour)
            //    {
            //        return allFoodWithUserOrderDetails;
            //    }
            //    else if (foodType == FoodTypeEnum.Dinner && DateTime.Now.Hour >= CustomDataConstants.DinnerTimeHour)
            //    {
            //        return allFoodWithUserOrderDetails;
            //    }
            //}

            allFoodWithUserOrderDetails = await contextCM.Foods
                .AsNoTracking()
                .Where(f => f.FoodTypeId == (int)foodType)
                .Where(f => f.IsAvailable)
                .Where(f => f.FoodAvailabilityDays.Any(fa =>
                (fa.DayOfWeek == dayOfWeek) &&
                (fa.WeekOfMonth == weekOfMonth)
                ))
                .Select(fo => new FoodDetails()
                {
                    Food = fo,
                    FoodCountInCart = fo.EmployeeCarts.Where(cf => cf.FoodId == fo.Id
                    && cf.EmployeeId == sessionData.UserIdOrZero
                     && cf.OutDateStatus == (int)CartFoodOutDateEnum.InOrder
                     && cf.OrderDate.Date == userSelected_dateTime.Date
                    ).Sum(cf => cf.Quantity)
                })
                .ToListAsync(cancellationToken);
            return allFoodWithUserOrderDetails;
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

        public async Task<List<FoodOrderDto>> GetFoodOrdersToday(int employeeId, FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {
            List<FoodOrderDto> foodOrders = new List<FoodOrderDto>();
            EncryptionDecryptions encryptionDecryptions = new EncryptionDecryptions();
            DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();
            //var foodOrders = await contextCM.FoodOrders
            //    .Include(fo => fo.FoodOrderFoodDetails.Where(fd => !fd.IsCanceled
            //    && fd.FoodTypeId == (int)foodTypeEnum && fd.EmployeeId == employeeId)
            //    .OrderBy(fo => fo.OrderDate)
            //    )
            //    .AsNoTracking()
            //    .Where(fo => fo.EmployeeId == employeeId
            //    && fo.FoodOrderFoodDetails.Any(fd =>
            //    fd.FoodTypeId == (int)foodTypeEnum
            //    && !fd.IsCanceled
            //    && fd.OrderDateCustom.Date == DateTime.Now.Date
            //    )
            //    )
            //    .ToListAsync(cancellationToken);
            foodOrders = await contextCM.FoodOrders
                //.Include(f => f.Food)
                //.Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date == DateTime.Now.Date
                && fo.EmployeeId == employeeId
                && !fo.IsCanceled
                && fo.Food.FoodTypeId == (int)foodTypeEnum
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
                    QrCodeText = encryptionDecryptions.EncryptString($"{fo.OrderID}-|-{employeeId}-|-{dateCalculationHelper.DateTimeToString(DateTime.Now)}"),
                })
                .ToListAsync(cancellationToken);
            //encryptionDecryptions.EncryptString($"{fo.OrderID}-|-{fo.Employee.EmployeeID}-|-{dateCalculationHelper.DateTimeToString(DateTime.Now)}"
            //foreach (var fo in foodOrders)
            //{
            //    //fo.QrCodeText = GetOrderQrData(fo.OrderID, employeeId);
            //    fo.QrCodeText = encryptionDecryptions.EncryptString($"{fo.OrderID}-|-{employeeId}-|-{dateCalculationHelper.DateTimeToString(DateTime.Now)}");
            //}

            return foodOrders;
        }

        public async Task<List<FoodOrderDto>> GetFoodOrdersAll(int employeeId, FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {
            EncryptionDecryptions encryptionDecryptions = new EncryptionDecryptions();
            DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();

            var foodOrders = await contextCM.FoodOrders
                //.Include(f => f.Food)
                //.Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date >= DateTime.Now.Date
                && fo.EmployeeId == employeeId
                && !fo.IsCanceled
                && fo.Food.FoodTypeId == (int)foodTypeEnum
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
                    QrCodeText = encryptionDecryptions.EncryptString($"{fo.OrderID}-|-{employeeId}-|-{dateCalculationHelper.DateTimeToString(DateTime.Now)}"),
                })
                .ToListAsync(cancellationToken);
            //foreach (var fo in foodOrders)
            //{
            //    //fo.QrCodeText = GetOrderQrData(fo.OrderID, employeeId);
            //    fo.QrCodeText = encryptionDecryptions.EncryptString($"{fo.OrderID}-|-{employeeId}-|-{dateCalculationHelper.DateTimeToString(DateTime.Now)}");
            //}
            return foodOrders;
        }

        //public string GetOrderQrData(string OrderId, int? EmpId)
        //{
        //    DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();
        //    var claims = new List<Claim>
        //                {
        //                    new Claim("EmpId", EmpId.ToString()??""),
        //                    new Claim("OrderID",OrderId),
        //                    new Claim("OrderDate", dateCalculationHelper.DateTimeToString(DateTime.Now)) // Add user role
        //                };
        //    return loginService.GenerateJSONWebToken(claims, DateTime.Now.AddHours(4));
        //}
        public async Task<List<EmployeeFoodOrdersTableDataModel>> GetFoodOrdersToday_Filter(FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken, string SearchVal = "")
        {
            var foodOrders = orderDataCaching.OrderCacheDataDictionary.Where(fo =>
                fo.Value.FoodType == (int)foodTypeEnum
                &&
                (fo.Value.EmployeeName.ToLower().Contains(SearchVal)
                || fo.Value.FoodOrderId.ToLower().Contains(SearchVal) || fo.Value.EmployeeCode.ToLower().Contains(SearchVal))
                )
                .Select(fo => fo.Value)
                .ToList();

            //var foodOrders = await contextCM.FoodOrders
            //    //.Include(f => f.Food)
            //    //.Include(f => f.Employee)
            //    .AsNoTracking()
            //    .Where(fo =>
            //    fo.OrderDateCustom.Date >= DateTime.Now.Date
            //    && fo.Food.FoodTypeId == (int)foodTypeEnum
            //    && fo.IsCanceled == false
            //    &&
            //    (fo.Employee.Name.ToLower().Contains(SearchVal) ||
            //    fo.Employee.EmployeeID.ToLower().Contains(SearchVal) ||
            //    fo.OrderID.ToLower().Contains(SearchVal)
            //    )
            //    )
            //    .OrderBy(fo => fo.IsCompleted)
            //    //.Take(10)
            //    .Select(fo => new EmployeeFoodOrdersTableDataModel()
            //    {
            //        FoodId = fo.Id,
            //        FoodOrderId = fo.OrderID,
            //        EmployeeId = fo.EmployeeId ?? 0,
            //        EmployeeCode = fo.Employee.EmployeeID,
            //        FoodName = fo.Food.Name,
            //        OrderDate = fo.OrderDateCustom,
            //        Quantity = fo.Quantity,
            //        TotalPrice = fo.TotalPrice,
            //        FoodType = fo.Food.FoodTypeId,
            //        EmployeeName = fo.Employee.Name,
            //        IsCompleted = fo.IsCompleted,

            //    })
            //    .ToListAsync(cancellationToken);
            return foodOrders;
        }

        public async Task<EmployeeFoodOrdersTableDataModel?> GetFoodOrdersToday_Single(CancellationToken cancellationToken, int EmpID, string OrderID)
        {
            var foodOrders = await contextCM.FoodOrders
            .AsNoTracking()
            .Where(fo =>
            fo.OrderDateCustom.Date == DateTime.Now.Date
            //&& fo.Food.FoodTypeId == (int)foodTypeEnum
            &&
            fo.IsCanceled == false
            && fo.EmployeeId == EmpID
            &&
            fo.OrderID.ToLower() == OrderID.ToLower()
            )
            .Select(fo => new EmployeeFoodOrdersTableDataModel()
            {
                FoodId = fo.Id,
                FoodOrderId = fo.OrderID,
                EmployeeId = fo.EmployeeId ?? 0,
                EmployeeCode = fo.Employee.EmployeeID,
                FoodName = fo.Food.Name,
                OrderDate = fo.OrderDateCustom,
                OrderDateS = fo.OrderDateCustom.Date.ToString(),
                Quantity = fo.Quantity,
                TotalPrice = fo.TotalPrice,
                FoodType = fo.Food.FoodTypeId,
                EmployeeName = fo.Employee.Name,
                IsCompleted = fo.IsCompleted,

            })
            .FirstOrDefaultAsync(cancellationToken);
            return foodOrders;
        }

        public async Task<List<EmployeeFoodOrdersTableDataModel>> GetFoodOrdersToday(FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {

            var foodOrders = await contextCM.FoodOrders
                //.Include(f => f.Food)
                //.Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo =>
                fo.OrderDateCustom.Date == DateTime.Now.Date
                && fo.Food.FoodTypeId == (int)foodTypeEnum
                && fo.IsCanceled == false
                //&&
                //(fo.Employee.Name.ToLower().Contains(SearchVal) ||
                //fo.Employee.EmployeeID.ToLower().Contains(SearchVal) ||
                //fo.OrderID.ToLower().Contains(SearchVal)
                //)
                )
                .OrderBy(fo => fo.IsCompleted)
                //.Take(10)
                .Select(fo => new EmployeeFoodOrdersTableDataModel()
                {
                    FoodId = fo.Id,
                    FoodOrderId = fo.OrderID,
                    EmployeeId = fo.EmployeeId ?? 0,
                    EmployeeCode = fo.Employee.EmployeeID,
                    FoodName = fo.Food.Name,
                    OrderDate = fo.OrderDateCustom,
                    Quantity = fo.Quantity,
                    TotalPrice = fo.TotalPrice,
                    FoodType = fo.Food.FoodTypeId,
                    EmployeeName = fo.Employee.Name,
                    IsCompleted = fo.IsCompleted,

                })
                .ToListAsync(cancellationToken);

            var onlyfoodOrderIds = foodOrders.Select(fo => fo.FoodOrderId).ToList();
            //await cache.RemoveAsync(foodOrders.Select(foodOrders => foodOrders.FoodOrderId).ToList(), cancellationToken);
            EmployeeFoodOrdersTableDataModel outobj = new EmployeeFoodOrdersTableDataModel();
            DateTime todayDate = DateTime.Now.Date;

            //get old order to be remove
            foreach (var item in orderDataCaching.OrderCacheDataDictionary)
            {
                if (item.Value.OrderDate.Date < todayDate)
                {
                    onlyfoodOrderIds.Add(item.Key);
                }
            }

            //remove all old and existing orders from cache
            foreach (var item in onlyfoodOrderIds)
            {
                orderDataCaching.OrderCacheDataDictionary.TryRemove(item, out outobj);
            }

            // add new data to catch
            foreach (var item in foodOrders)
            {
                orderDataCaching.OrderCacheDataDictionary.TryAdd(item.FoodOrderId, item);
            }


            return foodOrders;
        }


        public async Task<List<EmployeeFoodOrdersTableDataModel>> GetFoodOrdersOld_CU(CancellationToken cancellationToken, string SearchVal = "")
        {
            var foodOrders = await contextCM.FoodOrders
                //.Include(f => f.Food)
                //.Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date < DateTime.Now.Date
                && fo.IsCanceled == false
                //&& fo.OrderCompleteStatus == (int)OrderCompleteStatusEnum.Pending
                && (fo.Employee.Name.ToLower().Contains(SearchVal) || fo.Employee.EmployeeID.ToLower().Contains(SearchVal))
                )
                .Select(fo => new EmployeeFoodOrdersTableDataModel()
                {
                    FoodId = fo.Id,
                    FoodOrderId = fo.OrderID,
                    EmployeeId = fo.EmployeeId ?? 0,
                    EmployeeCode = fo.Employee.EmployeeID,
                    FoodName = fo.Food.Name,
                    OrderDate = fo.OrderDateCustom,
                    Quantity = fo.Quantity,
                    TotalPrice = fo.TotalPrice,
                    FoodType = fo.Food.FoodTypeId,
                    EmployeeName = fo.Employee.Name,
                    IsCompleted = fo.IsCompleted,

                })
                .ToListAsync(cancellationToken);

            return foodOrders;
        }

        public async Task<bool> CompleteFoodOrder(string foodorderID)
        {

            var oldfoodorder = orderDataCaching.OrderCacheDataDictionary.Where(fo => fo.Value.FoodOrderId == foodorderID).Select(fo => fo.Value).FirstOrDefault();
            if (oldfoodorder != null)
            {
                oldfoodorder.IsCompleted = true;
                orderDataCaching.OrderCacheDataDictionary.Remove(oldfoodorder.FoodOrderId, out _);
                orderDataCaching.OrderCacheDataDictionary.TryAdd(oldfoodorder.FoodOrderId, oldfoodorder);
            }
            return false;
        }

        public async Task<List<CanteenFoodDetailsDTOModel>> getCanteenUserFoodOrderGroupList(int foodType, CancellationToken cancellationToken)
        {
            var FoodlistGrouping = await contextCM.FoodOrders
                    .Include(f => f.Food)
                    .AsNoTracking()
                    .Where(f =>
                    f.Food.FoodTypeId == foodType
                    && f.OrderDateCustom.Date >= DateTime.Now.Date
                    && f.IsCanceled == false
                    )
                    .GroupBy(f => new { f.FoodId, f.OrderDateCustom.Date })
                    .Select(f => new CanteenFoodDetailsDTOModel()
                    {
                        Id = f.Max(fo => fo.Id),
                        Name = f.Max(fm => fm.Food.Name) ?? "",
                        OrderDate = f.Key.Date,
                        FoodTypeId = f.Max(fm => fm.Food.FoodTypeId),
                        Price = 0,
                        FoodQuantity = f.Sum(fo => fo.Quantity),
                        TotalCompleted = f.Where(fo => fo.IsCompleted).Sum(fo => fo.Quantity),
                        TotalUnCompleted = f.Where(fo => !fo.IsCompleted).Sum(fo => fo.Quantity),
                        //EmployId = f.Max(fo => fo.EmployeeId ?? 0),
                        //EmployName = f.Max(fo => fo.Employee.Name) ?? "",
                    })
                    .ToListAsync(cancellationToken);
            return FoodlistGrouping;
        }

        public async Task<List<ReportMonthsDDLDataModel>> GetMonthListForReports(CancellationToken cancellation)
        {
            var monthList = await contextCM.FoodOrders
                .AsNoTracking()
                .Where(fo => fo.IsCanceled == false)
                .GroupBy(fo => new { fo.OrderDateCustom.Year, fo.OrderDateCustom.Month })
                .Select(fo => new ReportMonthsDDLDataModel
                {
                    DDL_Id = $"{fo.Key.Year}_{fo.Key.Month}",
                    Values = $"{new DateTime(fo.Key.Year, fo.Key.Month, 1):MMMM yyyy}"
                })
                .Distinct()
                .ToListAsync(cancellation);
            return monthList;
        }

        internal async Task<List<CanteenOrdersReportTableViewDataModel>> GetOrderReport(DateTime fromDate, DateTime toDate, string orderStatusOptions, CancellationToken cancellationToken, bool OnlyNonSubsidiary = false)
        {
            bool isCompletedstatus = orderStatusOptions == "1" ? true : orderStatusOptions == "2" ? false : false;

            bool getall = false;

            if (orderStatusOptions == "3")
            {
                getall = true;
            }

            var reportlist = await contextCM.FoodOrders
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date >= fromDate.Date &&
                fo.OrderDateCustom.Date <= toDate.Date
                && fo.IsCanceled == false
                && (OnlyNonSubsidiary ? fo.TotalSubsidyPrice == 0 : fo.TotalSubsidyPrice != 0)
                && (fo.IsCompleted == isCompletedstatus || getall)
                )
                .Select(fo => fo)
                .GroupBy(fo => new { fo.OrderDateCustom.Date })
                .Select(fo => new CanteenOrdersReportTableViewDataModel()
                {
                    OrderDate = fo.Key.Date,
                    TotalOrderCount = fo.Count(),
                    TotalQuantity = fo.Sum(fo => fo.Quantity),
                    TotalEmployeeCount = fo.Select(fo => fo.EmployeeId).Distinct().Count(),
                    TotalPrice = fo.Sum(fo => fo.TotalPrice),
                    TotalEmployeePrice = fo.Sum(fo => fo.TotalEmployeePrice),
                    TotalSubsidyPrice = fo.Sum(fo => fo.TotalSubsidyPrice) + (OnlyNonSubsidiary ? 0 : 100)
                })
                .ToListAsync(cancellationToken);
            var total_data = new CanteenOrdersReportTableViewDataModel()
            {
                OrderDate = DateTime.Now,
                TotalOrderCount = reportlist.Sum(r => r.TotalOrderCount),
                TotalQuantity = reportlist.Sum(fo => fo.TotalQuantity),
                TotalEmployeeCount = reportlist.Sum(r => r.TotalEmployeeCount),
                TotalPrice = reportlist.Sum(r => r.TotalPrice),
                TotalEmployeePrice = reportlist.Sum(r => r.TotalEmployeePrice),
                TotalSubsidyPrice = reportlist.Sum(r => r.TotalSubsidyPrice)
            };
            reportlist.Add(total_data);
            return reportlist;
        }

        public async Task<List<FoodReportDetailsViewModel>> GetOrderReportByDate(DateTime date, string orderStatusOptions, CancellationToken cancellationToken, bool IncludeSubsidiary = false)
        {
            bool isCompletedstatus = orderStatusOptions == "1" ? true : orderStatusOptions == "2" ? false : false;
            bool getall = false;

            if (orderStatusOptions == "3")
            {
                getall = true;
            }


            var orders = await contextCM.FoodOrders
                .AsNoTracking()
                .Where(o => o.OrderDateCustom.Date == date.Date
                && o.IsCanceled == false
                && (IncludeSubsidiary ? o.TotalSubsidyPrice == 0 : o.TotalSubsidyPrice != 0)
                && (o.IsCompleted == isCompletedstatus || getall)
                )
                .Select(f => new FoodReportDetailsViewModel()
                {
                    EmployeeName = f.Employee.Name,
                    FoodName = f.FoodName ?? "",
                    FoodTypeName = f.Food.FoodType.Name.Substring(0, 1),
                    Quantity = f.Quantity,
                    TotalPrice = f.TotalPrice,
                    EmployeePrice = f.TotalEmployeePrice,
                    SubsidiaryPrice = f.TotalSubsidyPrice,
                }
                )
                .OrderBy(fo => fo.FoodTypeName).ThenBy(fo => fo.FoodName)
                .ToListAsync(cancellationToken);

            return orders;
        }



        public async Task<List<string>> GetTodayFoodNames(int foodType, CancellationToken cancellationToken)
        {
            var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            var weekOfMonth = GetWeekOfMonth(DateTime.Now);
            if (weekOfMonth == 5)
            {
                weekOfMonth = 1;
            }
            var allFoodWithUserOrderDetails = new List<Food>();

            var FoodNameList = await contextCM.Foods
                .AsNoTracking()
               .Where(f => f.FoodTypeId == foodType)
               .Where(f => f.IsAvailable)
               .Where(f => f.FoodAvailabilityDays.Any(fa =>
               (fa.DayOfWeek == dayOfWeek) &&
               (fa.WeekOfMonth == weekOfMonth)
               ))
               .Select(x => x.Name)
               .ToListAsync(cancellationToken);
            return FoodNameList;
        }
        public async Task<List<string?>> GetMyOrderTodayFoodNames(int foodType, int employeeId, CancellationToken cancellationToken)
        {
            var myFoodOrderNames = await contextCM.FoodOrders
             //.Include(f => f.Food)
             //.Include(f => f.Employee)
             .AsNoTracking()
             .Where(fo => fo.OrderDateCustom.Date == DateTime.Now.Date
             && fo.EmployeeId == employeeId
             && !fo.IsCanceled
             && fo.Food.FoodTypeId == foodType
             )
             //.OrderBy(fo => fo.OrderDateCustom)
             .Select(fo => fo.FoodName)
             .ToListAsync(cancellationToken);
            return myFoodOrderNames;

        }
        public async Task<List<EmployeeFeedback>> GetAllEmployeeFeedbacks()
        {
            return await contextCM.EmployeeFeedbacks
                .OrderByDescending(m => m.SubmittedAt)
                .ToListAsync();
        }

        public async Task SubmitEmployeeFeedbacks(int employeeID, string message, string employeeName)
        {

            contextCM.EmployeeFeedbacks.Add(new EmployeeFeedback
            {
                EmployeeId = employeeID,
                Message = message,
                Name = employeeName,
                Email = "",
                SubmittedAt = DateTime.Now
            });
            await contextCM.SaveChangesAsync();

        }
        public async Task<List<FoodOrder>> SearchOrdersByEmployee(string searchTerm)
        {
            return await contextCM.FoodOrders
                .Include(f => f.Employee)
                .Where(f =>
                    f.Employee.Name.Contains(searchTerm) ||
                    f.Employee.Id.ToString() == searchTerm
                )
                .ToListAsync();
        }
        public async Task<string?> GetEmployeeIdByUserIdAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return null;

            var employee = await contextCM.Employees
                .FirstOrDefaultAsync(ue => ue.Id == id);

            return employee?.EmployeeID;
        }

        public async Task<List<FoodDetails>> GetquickfoodsAsync(CancellationToken cancellationToken)
        {
            var result = await contextCM.Foods
                                        .Where(f => f.FoodTypeId == 4)
                                        .Select(f => new FoodDetails
                                        {
                                            Food = f,
                                            FoodCountInCart = 0 // Assuming no cart count for quick foods
                                        })
                                        .ToListAsync(cancellationToken);

            return result;
        }
        public async Task<List<WeeklyFoodList>> GetWeekWiseFoodlist(int weekNumber, CancellationToken cancellationToken, string? searchTerm = null)
        {
            var ffff = await contextCM.FoodAvailabilityDays
                //.Include(fo => fo.Food)
                .AsNoTracking()
                .Where(fo => fo.WeekOfMonth == weekNumber
                && fo.Food.IsAvailable
                )
                .GroupBy(fo => fo.DayOfWeek)
                .Select(g => new WeeklyFoodList
                {
                    DayOfWeek = ((DayOfWeek)g.Key).ToString(),
                    Foods = g.Select(x => x.Food).Distinct().ToList()
                })
                .ToListAsync(cancellationToken);

            return ffff;
        }

        public async Task<(int TodayTotal, int TodayTotalCompleted, int TodayTotalUnCompleted, int tomorrow, int all)> GetOrderCounts(CancellationToken cancellationToken)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            //var todayCounts = await contextCM.FoodOrders
            //    .AsNoTracking()
            //    .Where(o => !o.IsCanceled
            //    && o.OrderDateCustom.Date == today
            //    )
            //    .GroupBy(fo => fo.Id)
            //    .Select(fo => new
            //    {
            //        TotalCount = fo.Sum(f => f.Quantity),
            //        TotalCompleted = fo.Sum(f => f.IsCompleted ? f.Quantity : 0),
            //        TotalUnCompleted = fo.Sum(f => !f.IsCompleted ? f.Quantity : 0)
            //    })
            //    .FirstOrDefaultAsync(cancellationToken)
            //    ;
            int todayCounts_total = await contextCM.FoodOrders.AsNoTracking().CountAsync(o => !o.IsCanceled && o.OrderDateCustom.Date == today, cancellationToken: cancellationToken);
            int todayCounts_com = await contextCM.FoodOrders.AsNoTracking().CountAsync(o => !o.IsCanceled && o.IsCompleted && o.OrderDateCustom.Date == today, cancellationToken: cancellationToken);
            int todayCounts_uncom = await contextCM.FoodOrders.AsNoTracking().CountAsync(o => !o.IsCanceled && !o.IsCompleted && o.OrderDateCustom.Date == today, cancellationToken: cancellationToken);
            int tomorrowCount = await contextCM.FoodOrders.AsNoTracking().CountAsync(o => !o.IsCanceled && o.OrderDateCustom.Date == tomorrow, cancellationToken: cancellationToken);
            int allCount = await contextCM.FoodOrders.AsNoTracking().CountAsync(o => !o.IsCanceled && o.OrderDateCustom.Date >= DateTime.Now.Date, cancellationToken: cancellationToken);

            return (todayCounts_total, todayCounts_com, todayCounts_uncom, tomorrowCount, allCount);
        }

        public async Task<List<CanteenFoodDetailsDTOModel>> GetOrdersByDateAsync(DateTime date, FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken, bool showAllData)
        {
            List<CanteenFoodDetailsDTOModel> orders = new List<CanteenFoodDetailsDTOModel>();

            if (showAllData)
            {
                orders = await contextCM.FoodOrders
                 .Include(f => f.Food)
                 .AsNoTracking()
                 .Where(o =>
                 o.Food.FoodTypeId == (int)foodTypeEnum
                 && o.OrderDateCustom.Date >= DateTime.Now.Date
                 && !o.IsCanceled
                 )
                 .GroupBy(f => new { f.FoodId, f.OrderDateCustom.Date })
                 .Select(f => new CanteenFoodDetailsDTOModel()
                 {
                     Id = f.Max(fo => fo.Id),
                     Name = f.Max(fm => fm.Food.Name) ?? "",
                     OrderDate = f.Key.Date,
                     FoodTypeId = f.Max(fm => fm.Food.FoodTypeId),
                     Price = 0,
                     FoodQuantity = f.Sum(fo => fo.Quantity),
                     TotalCompleted = f.Where(fo => fo.IsCompleted).Sum(fo => fo.Quantity),
                     TotalUnCompleted = f.Where(fo => !fo.IsCompleted).Sum(fo => fo.Quantity)
                 })
                 .ToListAsync(cancellationToken);
            }
            else
            {
                orders = await contextCM.FoodOrders
                .Include(f => f.Food)
                .AsNoTracking()
                .Where(o =>
                o.Food.FoodTypeId == (int)foodTypeEnum
                && o.OrderDateCustom.Date == date.Date
                && !o.IsCanceled
                )
                .GroupBy(f => new { f.FoodId, f.OrderDateCustom.Date })
                .Select(f => new CanteenFoodDetailsDTOModel()
                {
                    Id = f.Max(fo => fo.Id),
                    Name = f.Max(fm => fm.Food.Name) ?? "",
                    OrderDate = f.Key.Date,
                    FoodTypeId = f.Max(fm => fm.Food.FoodTypeId),
                    Price = 0,
                    FoodQuantity = f.Sum(fo => fo.Quantity),
                    TotalCompleted = f.Where(fo => fo.IsCompleted).Sum(fo => fo.Quantity),
                    TotalUnCompleted = f.Where(fo => !fo.IsCompleted).Sum(fo => fo.Quantity)
                })
                .ToListAsync(cancellationToken);
            }

            return orders;
        }


    }
}
