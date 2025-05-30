﻿// <auto-generated />
using System;
using CanteenManage.CanteenRepository.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CanteenManage.Migrations
{
    [DbContext(typeof(CanteenManageDBContext))]
    [Migration("20250520051338_canteenDBMig20")]
    partial class canteenDBMig20
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("EmployFeedbacks");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EmployTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Admin",
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Description = "CanteenStaf",
                            Name = "CanteenStaf"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Employee",
                            Name = "Employee"
                        },
                        new
                        {
                            Id = 4,
                            Description = "committee members",
                            Name = "Committee Members"
                        });
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("EmployID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployTypeId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("EmployTypeId");

                    b.ToTable("Employes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "",
                            EmployID = "SD1265",
                            EmployTypeId = 3,
                            IsActive = true,
                            Name = "Ardhendu Sekhar Sahoo",
                            Password = "",
                            PhoneNumber = ""
                        },
                        new
                        {
                            Id = 2,
                            Email = "",
                            EmployID = "EMP002",
                            EmployTypeId = 3,
                            IsActive = true,
                            Name = "Rojalin ",
                            Password = "",
                            PhoneNumber = ""
                        },
                        new
                        {
                            Id = 3,
                            Email = "",
                            EmployID = "EMP003",
                            EmployTypeId = 2,
                            IsActive = true,
                            Name = "Satyajit",
                            Password = "",
                            PhoneNumber = ""
                        },
                        new
                        {
                            Id = 4,
                            Email = "",
                            EmployID = "EMP004",
                            EmployTypeId = 1,
                            IsActive = true,
                            Name = "Ardhendu Admin",
                            Password = "",
                            PhoneNumber = ""
                        },
                        new
                        {
                            Id = 5,
                            Email = "",
                            EmployID = "EMP005",
                            EmployTypeId = 4,
                            IsActive = true,
                            Name = "Ardhendu Member",
                            Password = "",
                            PhoneNumber = ""
                        });
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployeeCart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("FoodId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OutDateStatus")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("FoodId");

                    b.ToTable("EmployeeCarts");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.Food", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AvailableOnDay")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("EmployeePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FoodTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<decimal>("SubsidyPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("FoodTypeId");

                    b.ToTable("Foods");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Veg_Sandwich.jpg",
                            IsAvailable = true,
                            Name = "Veg Sandwich",
                            Price = 60m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 2,
                            AvailableOnDay = 0,
                            Description = "Dosa with Sambar & Chutney",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Masala_Dosa.jpg",
                            IsAvailable = true,
                            Name = "Masala Dosa",
                            Price = 80m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 3,
                            AvailableOnDay = 0,
                            Description = "Paratha with Curd & Pickle",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Paratha.jpg",
                            IsAvailable = true,
                            Name = "Paratha",
                            Price = 50m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 4,
                            AvailableOnDay = 0,
                            Description = "Poha with Sev & Lemon",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Poha.jpg",
                            IsAvailable = true,
                            Name = "Poha",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 5,
                            AvailableOnDay = 0,
                            Description = "Idli (4 pcs) with Sambar & Chutney",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Idli.jpg",
                            IsAvailable = true,
                            Name = "Idli",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 6,
                            AvailableOnDay = 0,
                            Description = "Aloo Puri (4 puris)",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Aloo Puri.jpg",
                            IsAvailable = true,
                            Name = "Aloo Puri",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 7,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Bread_Omelette.jpg",
                            IsAvailable = true,
                            Name = "Bread & Omelette",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 8,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 1,
                            ImageUrl = "Chai_Tea.jpg",
                            IsAvailable = true,
                            Name = "Chai (Tea)",
                            Price = 15m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 9,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Veg_Thali.jpg",
                            IsAvailable = true,
                            Name = "Veg Thali",
                            Price = 120m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 10,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Chole_Chawal.jpg",
                            IsAvailable = true,
                            Name = "Chole Chawal",
                            Price = 140m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 11,
                            AvailableOnDay = 0,
                            Description = "Veg Biryani with Raita",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Veg_Biryani.jpg",
                            IsAvailable = true,
                            Name = "Veg Biryani",
                            Price = 110m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 12,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Rajma_Chawal.jpg",
                            IsAvailable = true,
                            Name = "Rajma Chawal",
                            Price = 90m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 13,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Chicken_Thali.jpg",
                            IsAvailable = true,
                            Name = "Chicken Thali",
                            Price = 160m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 14,
                            AvailableOnDay = 0,
                            Description = "Egg Curry with Rice or 2 Rotis",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Egg_Thali.jpg",
                            IsAvailable = true,
                            Name = "Egg Curry with Rice",
                            Price = 100m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 15,
                            AvailableOnDay = 0,
                            Description = "Chicken Biryani with Raita",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Chicken_Biryani.jpg",
                            IsAvailable = true,
                            Name = "Chicken Biryani",
                            Price = 150m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 16,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Fish_Curry_Rice.jpg",
                            IsAvailable = true,
                            Name = "Fish Curry with Rice",
                            Price = 170m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 17,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Roti.jpg",
                            IsAvailable = true,
                            Name = "Roti",
                            Price = 10m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 18,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Curd.jpg",
                            IsAvailable = true,
                            Name = "Curd",
                            Price = 20m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 19,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Salad.jpg",
                            IsAvailable = true,
                            Name = "Salad",
                            Price = 15m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 20,
                            AvailableOnDay = 0,
                            Description = "Papad (Roasted/Fried)",
                            EmployeePrice = 0m,
                            FoodTypeId = 2,
                            ImageUrl = "Papad.jpg",
                            IsAvailable = true,
                            Name = "Papad",
                            Price = 10m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 21,
                            AvailableOnDay = 0,
                            Description = "Samosa (2 pcs)",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Samosa.jpg",
                            IsAvailable = true,
                            Name = "Samosa",
                            Price = 30m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 22,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Veg_Puff.jpg",
                            IsAvailable = true,
                            Name = "Veg Puff",
                            Price = 25m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 23,
                            AvailableOnDay = 0,
                            Description = "Bread Pakora (2 pcs)",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Bread_Pakora.jpg",
                            IsAvailable = true,
                            Name = "Bread Pakora",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 24,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Onion_Pakoda.jpg",
                            IsAvailable = true,
                            Name = "Onion Pakoda",
                            Price = 45m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 25,
                            AvailableOnDay = 0,
                            Description = "Aloo Tikki (2 pcs)",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Aloo_Tikki.jpg",
                            IsAvailable = true,
                            Name = "Aloo Tikki",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 26,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Maggi_Noodles.jpg",
                            IsAvailable = true,
                            Name = "Maggi Noodles",
                            Price = 30m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 27,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Pav_Bhaji.jpg",
                            IsAvailable = true,
                            Name = "Pav Bhaji",
                            Price = 70m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 28,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Roll_Veg.jpg",
                            IsAvailable = true,
                            Name = "Roll (Veg)",
                            Price = 40m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 29,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Non_Veg_Roll.jpg",
                            IsAvailable = true,
                            Name = "Non-Veg Roll",
                            Price = 50m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        },
                        new
                        {
                            Id = 30,
                            AvailableOnDay = 0,
                            Description = "",
                            EmployeePrice = 0m,
                            FoodTypeId = 3,
                            ImageUrl = "Chai.jpg",
                            IsAvailable = true,
                            Name = "Chai (Tea)",
                            Price = 15m,
                            Rating = 0.0,
                            SubsidyPrice = 0m
                        });
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodAvailabilityDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("int");

                    b.Property<int>("FoodId")
                        .HasColumnType("int");

                    b.Property<int>("WeekOfMonth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.ToTable("FoodAvailabilityDays");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int?>("FoodId")
                        .HasColumnType("int");

                    b.Property<string>("FoodName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderCompleteStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("OrderUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime>("RatingCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Review")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalEmployeePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalSubsidyPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("FoodId");

                    b.ToTable("FoodOrders");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodReviewDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("FoodId")
                        .HasColumnType("int");

                    b.Property<int>("TotalRating")
                        .HasColumnType("int");

                    b.Property<int>("TotalUserCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FoodId");

                    b.ToTable("FoodReviewDetails");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FoodTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Breakfast"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Lunch"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Evening Snacks"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Quick Food"
                        });
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployFeedback", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.Employee", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.EmployType", "EmployType")
                        .WithMany("Employes")
                        .HasForeignKey("EmployTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployType");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployeeCart", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CanteenManage.CanteenRepository.Models.Food", "Food")
                        .WithMany("EmployeeCarts")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Food");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.Food", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.FoodType", "FoodType")
                        .WithMany("Foods")
                        .HasForeignKey("FoodTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FoodType");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodAvailabilityDay", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.Food", "Food")
                        .WithMany("FoodAvailabilityDays")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodOrder", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CanteenManage.CanteenRepository.Models.Food", "Food")
                        .WithMany("FoodOrders")
                        .HasForeignKey("FoodId");

                    b.Navigation("Employee");

                    b.Navigation("Food");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodReviewDetails", b =>
                {
                    b.HasOne("CanteenManage.CanteenRepository.Models.Food", "Food")
                        .WithMany()
                        .HasForeignKey("FoodId");

                    b.Navigation("Food");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.EmployType", b =>
                {
                    b.Navigation("Employes");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.Food", b =>
                {
                    b.Navigation("EmployeeCarts");

                    b.Navigation("FoodAvailabilityDays");

                    b.Navigation("FoodOrders");
                });

            modelBuilder.Entity("CanteenManage.CanteenRepository.Models.FoodType", b =>
                {
                    b.Navigation("Foods");
                });
#pragma warning restore 612, 618
        }
    }
}
