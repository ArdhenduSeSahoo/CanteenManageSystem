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
        private readonly CanteenManageDBContext Context;
        public FoodListingService(CanteenManageDBContext canteenManageContext)
        {
            this.Context = canteenManageContext;
        }
        public async Task<List<FoodOrder>> GetFoodOrdersByUserId(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await Context.FoodOrders
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

        public async Task<List<EmployeeCart>> GetCartFoodOrdersByUser(int userId, int foodType, DateTime orderDateTime, CancellationToken cancellationToken)
        {
            var foodOrderByUser = await Context.EmployeeCarts
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
            var foodOrderByUser = await Context.EmployeeCarts
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
            var foodOrderByUsercount = await Context.EmployeeCarts
                .Where(fo => fo.EmployeeId == userId)
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

            allFoodWithUserOrderDetails = await Context.Foods
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

        public async Task<List<EmployFoodOrdersTableDataModel>> GetFoodOrdersToday(FoodTypeEnum foodTypeEnum, CancellationToken cancellationToken)
        {
            var foodOrders = await Context.FoodOrders
                .Include(f => f.Food)
                .Include(f => f.Employee)
                .Where(fo => fo.OrderDate.Date >= DateTime.Now.Date
                && fo.Food.FoodTypeId == (int)foodTypeEnum
                //&& fo.OrderCompleteStatus == (int)OrderCompleteStatusEnum.Pending
                )
                .Select(fo => new EmployFoodOrdersTableDataModel()
                {
                    FoodOrderId = fo.Id,
                    EmployeeId = fo.EmployeeId,
                    FoodName = fo.Food.Name,
                    OrderDate = fo.OrderDate,
                    Quantity = fo.Quantity,
                    TotalPrice = fo.TotalPrice,
                    FoodType = fo.Food.FoodTypeId,
                    EmployeeName = fo.Employee.Name,
                    OrderCompleteStatus = fo.OrderCompleteStatus

                })
                .ToListAsync(cancellationToken);
            foodOrders.Add(new EmployFoodOrdersTableDataModel()
            {

            });
            return foodOrders;
        }

        public async Task CompleteFoodOrder(int foodorderID)
        {
            await Context.FoodOrders.Where(fo => fo.Id == foodorderID)
                .ExecuteUpdateAsync(fo => fo.SetProperty(f => f.OrderCompleteStatus, (int)OrderCompleteStatusEnum.Complete));
            await Context.SaveChangesAsync();
        }

        public async Task<List<CanteenFoodDetailsDTOModel>> getFoodOrderGroupList(int foodType, CancellationToken cancellationToken)
        {
            var FoodlistGrouping = await Context.FoodOrders
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

        public async Task<List<ReportMonthsDDLDataModel>> GetMonthListForReports(CancellationToken cancellation)
        {
            var monthList = await Context.FoodOrders
                .GroupBy(fo => new { fo.OrderDate.Year, fo.OrderDate.Month })
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
            var reportlist = await Context.FoodOrders
                .Include(f => f.Food)
                .Where(fo => fo.OrderDate.Year == years && fo.OrderDate.Month == months)
                .GroupBy(fo => new { fo.OrderDate.Date })
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
    }
}
