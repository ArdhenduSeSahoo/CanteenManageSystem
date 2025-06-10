using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManage.Controllers.CommitteeMembers
{
    [Authorize(Roles = "CommitteeMember")]
    public class CommitteeMemberController : Controller
    {
        private readonly CanteenManageDBContext context;
        //private readonly IWebHostEnvironment env;
        private readonly ILogger<CommitteeMemberController> logger;
        public CommitteeMemberController(CanteenManageDBContext context, ILogger<CommitteeMemberController> logger)
        {
            this.context = context;
            this.logger = logger;
            //this.env = env;
        }

        public IActionResult CMDashboard()
        {
            CMDashboardViewDataModel cMDashboardViewDataModel = new CMDashboardViewDataModel();

            return View(cMDashboardViewDataModel);
        }

        public async Task<IActionResult> FoodList(string searchTerm, CancellationToken cancellationToken)
        {
            var query = context.Foods
                .Include(f => f.FoodType)
                .Include(f => f.FoodAvailabilityDays)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(f => f.Name.Contains(searchTerm));
            }

            var foodList = await query.ToListAsync(cancellationToken);
            return View(foodList);
        }

        [HttpPost("DeleteFood")]
        public async Task<IActionResult> DeleteFood(IFormCollection formData)
        {
            var foodId = formData["foodid"];
            try
            {
                if (!string.IsNullOrEmpty(foodId))
                {
                    var foodtodelete = await context.Foods.Where(fo => fo.Id == int.Parse(foodId)).FirstOrDefaultAsync();
                    if (foodtodelete != null)
                    {
                        try
                        {
                            string filePath = Path.Combine(ProjectFilePathConstants.getImagePath(), foodtodelete.ImageUrl);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("DeleteFood--" + ex);
                        }
                        context.Foods.Remove(foodtodelete);
                        await context.SaveChangesAsync();

                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting food item with ID: {FoodId}", foodId);
            }

            return this.RedirectToAction("FoodList");
        }
        public async Task<IActionResult> FoodEdit([FromQuery(Name = "foodid")] string foodId, CancellationToken cancellationToken)
        {
            var foodidis = foodId;
            FoodFormDataModel? food = new FoodFormDataModel();
            try
            {
                food = await context.Foods
                .Include(f => f.FoodType)
                .Include(f => f.FoodAvailabilityDays)
                .Select(f => new FoodFormDataModel()
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    EmployeePrice = f.EmployeePrice,
                    SubsidyPrice = f.SubsidyPrice,
                    FoodTypeId = f.FoodTypeId,
                    FoodType = f.FoodType,
                    IsAvailable = f.IsAvailable,
                    ImageUrl = f.ImageUrl,
                    Rating = f.Rating,
                    IsVegFood = f.IsVegFood ?? false,

                    MonDay = f.FoodAvailabilityDays.Where(fa => fa.DayOfWeek == 1).Count() >= 1,
                    TuesDay = f.FoodAvailabilityDays.Where(fa => fa.DayOfWeek == 2).Count() >= 1,
                    WednesDay = f.FoodAvailabilityDays.Where(fa => fa.DayOfWeek == 3).Count() >= 1,
                    ThusDay = f.FoodAvailabilityDays.Where(fa => fa.DayOfWeek == 4).Count() >= 1,
                    FriDay = f.FoodAvailabilityDays.Where(fa => fa.DayOfWeek == 5).Count() >= 1,

                    WeekOneAndFive = f.FoodAvailabilityDays.Where(fa => fa.WeekOfMonth == 1 || fa.WeekOfMonth == 5).Count() >= 1,
                    WeekTwo = f.FoodAvailabilityDays.Where(fa => fa.WeekOfMonth == 2).Count() >= 1,
                    WeekThree = f.FoodAvailabilityDays.Where(fa => fa.WeekOfMonth == 3).Count() >= 1,
                    WeekFour = f.FoodAvailabilityDays.Where(fa => fa.WeekOfMonth == 4).Count() >= 1

                })
                .FirstOrDefaultAsync(f => f.Id == int.Parse(foodidis), cancellationToken)
                ;

            }
            catch (Exception ex)
            {
                logger.LogError("FoodEdit---" + ex);
            }
            food.AllFoodType = await context.FoodTypes.ToListAsync(cancellationToken);
            return View(food);
        }

        //[HttpPost("FoodUpdate")]
        [HttpPost]
        public async Task<IActionResult> FoodEdit(FoodFormDataModel foodFormDataModel)
        {


            if (string.IsNullOrEmpty(foodFormDataModel.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                foodFormDataModel.AllFoodType = await context.FoodTypes.ToListAsync();
                return View(foodFormDataModel);
            }
            try
            {
                if (foodFormDataModel.FoodImage != null && !string.IsNullOrEmpty(foodFormDataModel.ImageUrl))
                {
                    try
                    {

                        string filePath = Path.Combine(ProjectFilePathConstants.getImagePath(), foodFormDataModel.ImageUrl);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, 1024))
                        {
                            foodFormDataModel.FoodImage.CopyTo(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        foodFormDataModel.ImageUrl = "DefaultFood.png";
                        logger.LogError(ex, "Error in File upload");
                    }
                }
                if (string.IsNullOrEmpty(foodFormDataModel.ImageUrl))
                {
                    foodFormDataModel.ImageUrl = "DefaultFood.png";
                }

                var food_to_update = new Food()
                {
                    Id = foodFormDataModel.Id,
                    Name = foodFormDataModel.Name,
                    Description = foodFormDataModel.Description,
                    EmployeePrice = foodFormDataModel.EmployeePrice,
                    SubsidyPrice = foodFormDataModel.SubsidyPrice,
                    Price = foodFormDataModel.EmployeePrice + foodFormDataModel.SubsidyPrice,
                    FoodTypeId = foodFormDataModel.FoodTypeId,
                    IsAvailable = foodFormDataModel.IsAvailable,
                    ImageUrl = foodFormDataModel.ImageUrl,
                    Rating = foodFormDataModel.Rating,
                    IsVegFood = foodFormDataModel.IsVegFood
                };

                context.Foods.Update(food_to_update);

                var foodAvailabilityDays = await context.FoodAvailabilityDays.Where(fa => fa.FoodId == food_to_update.Id).ToListAsync();
                context.FoodAvailabilityDays.RemoveRange(foodAvailabilityDays);
                if (foodFormDataModel.WeekOneAndFive)
                {
                    context.FoodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 1));
                }
                if (foodFormDataModel.WeekTwo)
                {
                    context.FoodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 2));
                }
                if (foodFormDataModel.WeekThree)
                {
                    context.FoodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 3));
                }
                if (foodFormDataModel.WeekFour)
                {
                    context.FoodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 4));
                }

                await context.SaveChangesAsync();
                //if (model.ProfileImage != null)
                //{
                //    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "FoodImages");
                //    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        model.ProfileImage.CopyTo(fileStream);
                //    }
                //}

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "FoodEdit");
            }
            return this.RedirectToAction("FoodList");
        }
        public async Task<IActionResult> CreateFood()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(FoodFormDataModel foodFormDataModel)
        {
            if (string.IsNullOrEmpty(foodFormDataModel.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                foodFormDataModel.AllFoodType = await context.FoodTypes.ToListAsync();
                return View(foodFormDataModel);
            }
            try
            {
                if (foodFormDataModel.FoodImage != null && !string.IsNullOrEmpty(foodFormDataModel.ImageUrl))
                {
                    try
                    {
                        string filePath = Path.Combine(ProjectFilePathConstants.getImagePath(), foodFormDataModel.ImageUrl);
                        using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            logger.LogError("Error fetching in action CreateFood ");
                            foodFormDataModel.FoodImage.CopyTo(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Error fetching in action CreateFood ");
                        foodFormDataModel.ImageUrl = "DefaultFood.png";
                    }
                }
                if (string.IsNullOrEmpty(foodFormDataModel.ImageUrl))
                {
                    foodFormDataModel.ImageUrl = "DefaultFood.png";
                }
                List<FoodAvailabilityDay> foodAvailabilityDays = new List<FoodAvailabilityDay>();

                if (foodFormDataModel.WeekOneAndFive)
                {
                    foodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 1));
                }
                if (foodFormDataModel.WeekTwo)
                {
                    foodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 2));
                }
                if (foodFormDataModel.WeekThree)
                {
                    foodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 3));
                }
                if (foodFormDataModel.WeekFour)
                {
                    foodAvailabilityDays.AddRange(
                    getListOfSelectedDayOfWeek(foodFormDataModel, foodFormDataModel.Id, 4));
                }

                var food_to_create = new Food()
                {
                    Name = foodFormDataModel.Name,
                    Description = foodFormDataModel.Description,
                    EmployeePrice = foodFormDataModel.EmployeePrice,
                    SubsidyPrice = foodFormDataModel.SubsidyPrice,
                    Price = foodFormDataModel.EmployeePrice + foodFormDataModel.SubsidyPrice,
                    FoodTypeId = foodFormDataModel.FoodTypeId,
                    IsAvailable = foodFormDataModel.IsAvailable,
                    ImageUrl = foodFormDataModel.ImageUrl,
                    Rating = foodFormDataModel.Rating,
                    FoodAvailabilityDays = foodAvailabilityDays,
                };
                context.Foods.Add(food_to_create);
                await context.SaveChangesAsync();
                return this.RedirectToAction("FoodList");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                logger.LogError(ex, "Error fetching in action CreateFood ");
            }
            return View(foodFormDataModel);
        }

        public List<FoodAvailabilityDay> getListOfSelectedDayOfWeek(FoodFormDataModel foodFormDataModel, int foodId, int weekId)
        {
            List<FoodAvailabilityDay> foodAvailabilityDays = new List<FoodAvailabilityDay>();
            if (foodFormDataModel.MonDay)
            {
                foodAvailabilityDays.Add(new FoodAvailabilityDay()
                {
                    DayOfWeek = 1,
                    WeekOfMonth = weekId,
                    FoodId = foodId
                });
            }
            if (foodFormDataModel.TuesDay)
            {
                foodAvailabilityDays.Add(new FoodAvailabilityDay()
                {
                    DayOfWeek = 2,
                    WeekOfMonth = weekId,
                    FoodId = foodId
                });
            }
            if (foodFormDataModel.WednesDay)
            {
                foodAvailabilityDays.Add(new FoodAvailabilityDay()
                {
                    DayOfWeek = 3,
                    WeekOfMonth = weekId,
                    FoodId = foodId
                });
            }
            if (foodFormDataModel.ThusDay)
            {
                foodAvailabilityDays.Add(new FoodAvailabilityDay()
                {
                    DayOfWeek = 4,
                    WeekOfMonth = weekId,
                    FoodId = foodId
                });
            }
            if (foodFormDataModel.FriDay)
            {
                foodAvailabilityDays.Add(new FoodAvailabilityDay()
                {
                    DayOfWeek = 5,
                    WeekOfMonth = weekId,
                    FoodId = foodId
                });
            }
            return foodAvailabilityDays;
        }

        public async Task<IActionResult> FoodOrderList()
        {
            var foodTypeList = await context.FoodTypes.ToListAsync();
            return View(foodTypeList);
        }
    }

}
