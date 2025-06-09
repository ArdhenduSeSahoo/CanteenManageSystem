using System.Threading;
using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.Services
{
    public class FoodListingService
    {
        private readonly CanteenManageDBContext contextCM;
        public FoodListingService(CanteenManageDBContext canteenManageContext)
        {
            this.contextCM = canteenManageContext;
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


        public async Task<List<Food>> GetAllFoodList(int foodType, List<EmployeeCart> foodOrdersByUser, CancellationToken cancellationToken, DateTime userSelected_dateTime)
        {
            var dayOfWeek = (int)userSelected_dateTime.DayOfWeek;
            var weekOfMonth = GetWeekOfMonth(userSelected_dateTime);

            //replacing weekOfMonth with 1 if it is 5 because week 5 and 1 are same
            if (weekOfMonth == 5)
            {
                weekOfMonth = 1;
            }
            var allFoodWithUserOrderDetails = new List<Food>();
            if (userSelected_dateTime.Date.Day == DateTime.Now.Date.Day)
            {
                if (foodType == (int)FoodTypeEnum.Breakfast && DateTime.Now.Hour >= CustomDataConstants.BreakfastTimeHour)
                {
                    return allFoodWithUserOrderDetails;
                }
                else if (foodType == (int)FoodTypeEnum.Lunch && DateTime.Now.Hour >= CustomDataConstants.LunchTimeHour)
                {
                    return allFoodWithUserOrderDetails;
                }
                else if (foodType == (int)FoodTypeEnum.Snacks && DateTime.Now.Hour >= CustomDataConstants.SnacksTimeHour)
                {
                    return allFoodWithUserOrderDetails;
                }
            }

            allFoodWithUserOrderDetails = await contextCM.Foods
               .Include(f => f.EmployeeCarts.Where(fo => foodOrdersByUser.Select(fo => fo.Id).Contains(fo.Id)))
               //.Include(f => f.FoodAvailabilityDays)
               .Where(f => f.FoodTypeId == foodType)
               .Where(f => f.IsAvailable)
               .Where(f => f.FoodAvailabilityDays.Any(fa =>
               (fa.DayOfWeek == dayOfWeek) &&
               (fa.WeekOfMonth == weekOfMonth)
               ))
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

        public async Task<List<FoodOrder>> GetFoodOrdersToday(int employeeId, FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {
            List<FoodOrder> foodOrders = new List<FoodOrder>();
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
                ).ToListAsync(cancellationToken);

            return foodOrders;
        }

        public async Task<List<FoodOrder>> GetFoodOrdersAll(int employeeId, FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {
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
            //    //&& fd.OrderDateCustom.Date == DateTime.Now.Date
            //    )
            //    )
            //    .OrderBy(fo => fo.OrderDate)
            //    .ToListAsync(cancellationToken);
            var foodOrders = await contextCM.FoodOrders
                //.Include(f => f.Food)
                //.Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date >= DateTime.Now.Date
                && fo.EmployeeId == employeeId
                && !fo.IsCanceled
                && fo.Food.FoodTypeId == (int)foodTypeEnum
                ).ToListAsync(cancellationToken);
            return foodOrders;
        }
        public async Task<List<EmployeeFoodOrdersTableDataModel>> GetFoodOrdersToday_CU(FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken, string SearchVal = "")
        {
            var foodOrders = await contextCM.FoodOrders
                .Include(f => f.Food)
                .Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo =>
                fo.OrderDateCustom.Date == DateTime.Now.Date
                && fo.Food.FoodTypeId == (int)foodTypeEnum
                && fo.IsCanceled == false
                &&
                (fo.Employee.Name.ToLower().Contains(SearchVal) ||
                fo.Employee.EmployID.ToLower().Contains(SearchVal) ||
                fo.OrderID.ToLower().Contains(SearchVal)
                )
                )
                .OrderBy(fo => fo.Id)
                .Take(10)
                .Select(fo => new EmployeeFoodOrdersTableDataModel()
                {
                    FoodId = fo.Id,
                    FoodOrderId = fo.OrderID,
                    EmployeeId = fo.EmployeeId ?? 0,
                    EmployeeCode = fo.Employee.EmployID,
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

        public async Task<List<EmployeeFoodOrdersTableDataModel>> GetFoodOrdersOld_CU(CancellationToken cancellationToken, string SearchVal = "")
        {
            var foodOrders = await contextCM.FoodOrders
                .Include(f => f.Food)
                .Include(f => f.Employee)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Date < DateTime.Now.Date
                && fo.IsCanceled == false
                //&& fo.OrderCompleteStatus == (int)OrderCompleteStatusEnum.Pending
                && (fo.Employee.Name.ToLower().Contains(SearchVal) || fo.Employee.EmployID.ToLower().Contains(SearchVal))
                )
                .Select(fo => new EmployeeFoodOrdersTableDataModel()
                {
                    FoodId = fo.Id,
                    FoodOrderId = fo.OrderID,
                    EmployeeId = fo.EmployeeId ?? 0,
                    EmployeeCode = fo.Employee.EmployID,
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
            try
            {
                await contextCM.FoodOrders.Where(fo => fo.OrderID == foodorderID)
    .ExecuteUpdateAsync(fo => fo.SetProperty(f => f.IsCompleted, true));
                await contextCM.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
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

        internal async Task<List<CanteenOrdersReportTableViewDataModel>> GetCanteenOrderReportData(int months, int years, CancellationToken cancellationToken)
        {
            var reportlist = await contextCM.FoodOrders
                .Include(f => f.Food)
                .AsNoTracking()
                .Where(fo => fo.OrderDateCustom.Year == years &&
                fo.OrderDateCustom.Month == months
                && fo.IsCanceled == false
                && fo.IsCompleted == true
                )
                .GroupBy(fo => new { fo.OrderDateCustom.Date })
                .Select(fo => new CanteenOrdersReportTableViewDataModel()
                {
                    OrderDate = fo.Key.Date,
                    TotalOrderCount = fo.Sum(fo => fo.Quantity),
                    TotalEmployeeCount = fo.Select(fo => fo.EmployeeId).Distinct().Count(),
                    TotalPrice = fo.Sum(fo => fo.TotalPrice),
                    TotalEmployeePrice = fo.Sum(fo => fo.TotalEmployeePrice),
                    TotalSubsidyPrice = fo.Sum(fo => fo.TotalSubsidyPrice)
                })
                .ToListAsync(cancellationToken);
            var total_data = new CanteenOrdersReportTableViewDataModel()
            {
                OrderDate = DateTime.Now,
                TotalOrderCount = reportlist.Sum(r => r.TotalOrderCount),
                TotalEmployeeCount = reportlist.Sum(r => r.TotalEmployeeCount),
                TotalPrice = reportlist.Sum(r => r.TotalPrice),
                TotalEmployeePrice = reportlist.Sum(r => r.TotalEmployeePrice),
                TotalSubsidyPrice = reportlist.Sum(r => r.TotalSubsidyPrice)
            };
            reportlist.Add(total_data);
            return reportlist;
        }

        //Abinash
        public async Task<FoodReportViewModel> GetCanteenOrderReportDataByDateRange(DateTime date, CancellationToken cancellationToken)
        {
            var orders = await contextCM.FoodOrders
                .Include(f => f.Food)
                .AsNoTracking()
                .Where(o => o.OrderDateCustom.Date == date.Date && o.IsCanceled == false)
                .ToListAsync(cancellationToken);

            return new FoodReportViewModel { FoodOrders = orders };
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

            return employee?.EmployID;
        }

        public async Task<List<Food>> GetquickfoodsAsync()
        {
            var result = await contextCM.Foods
                                        .Where(f => f.FoodTypeId == 4)
                                        .ToListAsync();

            return result;
        }
        public async Task<List<WeeklyFoodList>> GetWeekWiseFoodlist(int weekNumber, CancellationToken cancellationToken, string? searchTerm = null)
        {
            var ffff = await contextCM.FoodAvailabilityDays
                .Include(fo => fo.Food)
                .Where(fo => fo.WeekOfMonth == weekNumber)
                .GroupBy(fo => fo.DayOfWeek)
                .Select(g => new WeeklyFoodList
                {
                    DayOfWeek = ((DayOfWeek)g.Key).ToString(),
                    Foods = g.Select(x => x.Food).Distinct().ToList()
                })
                .ToListAsync(cancellationToken);

            return ffff;
        }




    }
}
