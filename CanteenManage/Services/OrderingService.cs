using CanteenManage.Models;
using CanteenManage.Repo.Contexts;
using CanteenManage.Repo.Models;
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

        public async Task<List<FoodOrder>> GetFoodOrdersByUserId(int userId, int foodType, DateTime orderDateTime)
        {
            var foodOrderByUser = await canteenManageContext.FoodOrders
               .Include(f => f.Food)
               .Where(fo => fo.EmployeId == userId
               &&
               fo.OrderDate.Date == orderDateTime.Date
               )
               .Where(fo =>
               fo.Food.FoodTypeId == foodType
               )
               .ToListAsync();
            return foodOrderByUser;
        }


        public async Task<List<Food>> GetFoodOrdersByUserId(int foodType, List<FoodOrder> foodOrdersByUser)
        {

            var allFoodWithUserOrderDetails = await canteenManageContext.Foods
               .Include(f => f.FoodOrders.Where(fo => foodOrdersByUser.Select(fo => fo.Id).Contains(fo.Id)))
               .Where(f => f.FoodTypeId == foodType)
               .ToListAsync();
            return allFoodWithUserOrderDetails;


        }

        public async Task<List<CanteenFoodDetailsDTOModel>> getFoodOrderGroupList(int foodType)
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
                    })
                    .ToListAsync();
            return FoodlistGrouping;
        }
    }
}
