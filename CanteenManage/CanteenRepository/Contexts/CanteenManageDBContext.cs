using CanteenManage.CanteenRepository.Models;
using Microsoft.EntityFrameworkCore;

namespace CanteenManage.CanteenRepository.Contexts
{
    public class CanteenManageDBContext : DbContext
    {
        public CanteenManageDBContext(DbContextOptions<CanteenManageDBContext> options) : base(options)
        {
        }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodType> FoodTypes { get; set; }
        public DbSet<FoodOrder> FoodOrders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<FoodReviewDetails> FoodReviewDetails { get; set; }
        public DbSet<FoodAvailabilityDay> FoodAvailabilityDays { get; set; }
        public DbSet<EmployeeFeedback> EmployeeFeedbacks { get; set; }
        public DbSet<EmployeeCart> EmployeeCarts { get; set; }
        public DbSet<FoodOrderFoodDetail> FoodOrderFoodDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CantenManageContext).Assembly);

            modelBuilder.Entity<FoodType>(ft =>
            {
                ft.HasData(new FoodType
                {
                    Id = 1,
                    Name = "Breakfast"
                },
                new FoodType
                {
                    Id = 2,
                    Name = "Lunch"
                },
                new FoodType
                {
                    Id = 3,
                    Name = "Evening Snacks"
                },
                new FoodType
                {
                    Id = 4,
                    Name = "Quick Food"
                }
                );
            });
            modelBuilder.Entity<EmployeeType>(et =>
            {
                et.HasData(new EmployeeType
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Admin"
                },
                new EmployeeType
                {
                    Id = 2,
                    Name = "CanteenStaf",
                    Description = "CanteenStaf"
                },
                new EmployeeType
                {
                    Id = 3,
                    Name = "Employee",
                    Description = "Employee"
                },
                new EmployeeType
                {
                    Id = 4,
                    Name = "Committee Members",
                    Description = "committee members"
                }
                );
            });
        }
    }
}
