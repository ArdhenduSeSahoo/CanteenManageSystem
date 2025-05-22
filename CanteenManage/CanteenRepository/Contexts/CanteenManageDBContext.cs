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
        public DbSet<Employee> Employes { get; set; }
        public DbSet<EmployType> EmployTypes { get; set; }
        public DbSet<FoodReviewDetails> FoodReviewDetails { get; set; }
        public DbSet<FoodAvailabilityDay> FoodAvailabilityDays { get; set; }
        public DbSet<EmployFeedback> EmployFeedbacks { get; set; }
        public DbSet<EmployeeCart> EmployeeCarts { get; set; }
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
            modelBuilder.Entity<EmployType>(et =>
            {
                et.HasData(new EmployType
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Admin"
                },
                new EmployType
                {
                    Id = 2,
                    Name = "CanteenStaf",
                    Description = "CanteenStaf"
                },
                new EmployType
                {
                    Id = 3,
                    Name = "Employee",
                    Description = "Employee"
                },
                new EmployType
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
