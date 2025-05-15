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
            modelBuilder.Entity<Food>(fo =>
            {
                fo.HasData(
                new Food
                {
                    Id = 1,
                    Name = "Veg Sandwich",
                    Description = "",
                    Price = 60,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Veg_Sandwich.jpg"
                },
                new Food
                {
                    Id = 2,
                    Name = "Masala Dosa",
                    Description = "Dosa with Sambar & Chutney",
                    Price = 80,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Masala_Dosa.jpg"
                },
                new Food
                {
                    Id = 3,
                    Name = "Paratha",
                    Description = "Paratha with Curd & Pickle",
                    Price = 50,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Paratha.jpg"
                },
                new Food
                {
                    Id = 4,
                    Name = "Poha",
                    Description = "Poha with Sev & Lemon",
                    Price = 40,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Poha.jpg"
                },
                new Food
                {
                    Id = 5,
                    Name = "Idli",
                    Description = "Idli (4 pcs) with Sambar & Chutney",
                    Price = 40,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Idli.jpg"
                },
                new Food
                {
                    Id = 6,
                    Name = "Aloo Puri",
                    Description = "Aloo Puri (4 puris)",
                    Price = 40,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Aloo Puri.jpg"
                },
                new Food
                {
                    Id = 7,
                    Name = "Bread & Omelette",
                    Description = "",
                    Price = 40,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Bread_Omelette.jpg"
                },
                new Food
                {
                    Id = 8,
                    Name = "Chai (Tea)",
                    Description = "",
                    Price = 15,
                    FoodTypeId = 1,
                    IsAvailable = true,
                    ImageUrl = "Chai_Tea.jpg"
                },
                new Food
                {
                    Id = 9,
                    Name = "Veg Thali",
                    Description = "",
                    Price = 120,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Veg_Thali.jpg"
                },
                new Food
                {
                    Id = 10,
                    Name = "Chole Chawal",
                    Description = "",
                    Price = 140,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Chole_Chawal.jpg"
                },
                new Food
                {
                    Id = 11,
                    Name = "Veg Biryani",
                    Description = "Veg Biryani with Raita",
                    Price = 110,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Veg_Biryani.jpg"
                },
                new Food
                {
                    Id = 12,
                    Name = "Rajma Chawal",
                    Description = "",
                    Price = 90,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Rajma_Chawal.jpg"
                },
                new Food
                {
                    Id = 13,
                    Name = "Chicken Thali",
                    Description = "",
                    Price = 160,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Chicken_Thali.jpg"
                },
                new Food
                {
                    Id = 14,
                    Name = "Egg Curry with Rice",
                    Description = "Egg Curry with Rice or 2 Rotis",
                    Price = 100,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Egg_Thali.jpg"
                },
                new Food
                {
                    Id = 15,
                    Name = "Chicken Biryani",
                    Description = "Chicken Biryani with Raita",
                    Price = 150,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Chicken_Biryani.jpg"
                },
                new Food
                {
                    Id = 16,
                    Name = "Fish Curry with Rice",
                    Description = "",
                    Price = 170,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Fish_Curry_Rice.jpg"
                },
                new Food
                {
                    Id = 17,
                    Name = "Roti",
                    Description = "",
                    Price = 10,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Roti.jpg"
                },
                new Food
                {
                    Id = 18,
                    Name = "Curd",
                    Description = "",
                    Price = 20,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Curd.jpg"
                },
                new Food
                {
                    Id = 19,
                    Name = "Salad",
                    Description = "",
                    Price = 15,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Salad.jpg"
                },
                new Food
                {
                    Id = 20,
                    Name = "Papad",
                    Description = "Papad (Roasted/Fried)",
                    Price = 10,
                    FoodTypeId = 2,
                    IsAvailable = true,
                    ImageUrl = "Papad.jpg"
                },
                new Food
                {
                    Id = 21,
                    Name = "Samosa",
                    Description = "Samosa (2 pcs)",
                    Price = 30,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Samosa.jpg"
                },
                new Food
                {
                    Id = 22,
                    Name = "Veg Puff",
                    Description = "",
                    Price = 25,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Veg_Puff.jpg"
                },
                new Food
                {
                    Id = 23,
                    Name = "Bread Pakora",
                    Description = "Bread Pakora (2 pcs)",
                    Price = 40,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Bread_Pakora.jpg"
                },
                new Food
                {
                    Id = 24,
                    Name = "Onion Pakoda",
                    Description = "",
                    Price = 45,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Onion_Pakoda.jpg"
                },
                new Food
                {
                    Id = 25,
                    Name = "Aloo Tikki",
                    Description = "Aloo Tikki (2 pcs)",
                    Price = 40,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Aloo_Tikki.jpg"
                },
                new Food
                {
                    Id = 26,
                    Name = "Maggi Noodles",
                    Description = "",
                    Price = 30,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Maggi_Noodles.jpg"
                },
                new Food
                {
                    Id = 27,
                    Name = "Pav Bhaji",
                    Description = "",
                    Price = 70,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Pav_Bhaji.jpg"
                },
                new Food
                {
                    Id = 28,
                    Name = "Roll (Veg)",
                    Description = "",
                    Price = 40,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Roll_Veg.jpg"
                },
                new Food
                {
                    Id = 29,
                    Name = "Non-Veg Roll",
                    Description = "",
                    Price = 50,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Non_Veg_Roll.jpg"
                },
                new Food
                {
                    Id = 30,
                    Name = "Chai (Tea)",
                    Description = "",
                    Price = 15,
                    FoodTypeId = 3,
                    IsAvailable = true,
                    ImageUrl = "Chai.jpg"
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
            modelBuilder.Entity<Employee>(e =>
            {
                e.HasData(
                    new Employee
                    {
                        Id = 1,
                        Name = "Ardhendu Sekhar Sahoo",
                        EmployTypeId = 3,
                        EmployID = "SD1265",
                        Password = "",
                        Email = "",
                        PhoneNumber = "",
                        IsActive = true
                    }
                    , new Employee
                    {
                        Id = 2,
                        Name = "Rojalin ",
                        EmployTypeId = 3,
                        EmployID = "EMP002",
                        Password = "",
                        Email = "",
                        PhoneNumber = "",
                        IsActive = true
                    },
                    new Employee
                    {
                        Id = 3,
                        Name = "Satyajit",
                        EmployTypeId = 2,
                        EmployID = "EMP003",
                        Password = "",
                        Email = "",
                        PhoneNumber = "",
                        IsActive = true
                    },
                    new Employee
                    {
                        Id = 4,
                        Name = "Ardhendu Admin",
                        EmployTypeId = 1,
                        EmployID = "EMP004",
                        Password = "",
                        Email = "",
                        PhoneNumber = "",
                        IsActive = true
                    },
                    new Employee
                    {
                        Id = 5,
                        Name = "Ardhendu Member",
                        EmployTypeId = 4,
                        EmployID = "EMP005",
                        Password = "",
                        Email = "",
                        PhoneNumber = "",
                        IsActive = true
                    }
                    );
            });

        }
    }
}
