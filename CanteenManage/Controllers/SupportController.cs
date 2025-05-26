using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class SupportController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;

        public SupportController(FoodListingService foodListingService, UtilityServices utilityServices)
        {
            this.foodListingService = foodListingService;
            this.utilityServices = utilityServices;
        }
        public async Task<IActionResult> Support()
        {
            var feedbackEntities = await foodListingService.GetAllEmployeeFeedbacks();

            var feedbackViewModels = feedbackEntities.Select(f => new EmployeeFeedbacks
            {
                Name = f.Name,
                Email = f.Email,
                Message = f.Message,
                SubmittedAt = f.SubmittedAt
            }).ToList();
            EmployeeFeedbackViewModel employeeFeedbackViewModel = new EmployeeFeedbackViewModel();
            employeeFeedbackViewModel.EmployeeFeedbacks = feedbackViewModels;
            return View(employeeFeedbackViewModel);
        }
        public IActionResult Submit(bool feedbackSubmitted = false)
        {
            EmployeeFeedbackViewModel employeeFeedbackViewModel = new EmployeeFeedbackViewModel();
            //SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            //
            employeeFeedbackViewModel.FeedbackSubmitted = feedbackSubmitted;
            return View(employeeFeedbackViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(IFormCollection formcollect)
        {
            var message = formcollect["Message"].ToString();
            var name = formcollect["Name"].ToString();
            var email = formcollect["Email"].ToString();
            try
            {
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                if (!string.IsNullOrEmpty(message))
                {
                    await foodListingService.SubmitEmployeeFeedbacks(sessionDataModel.UserIdOrZero, message, sessionDataModel.UserName ?? "");
                    return this.RedirectToAction("Submit", new { feedbackSubmitted = true });
                }

            }
            catch (Exception ex)
            {

            }
            //if (ModelState.IsValid)
            //{
            //    string userIdStr = HttpContext.Session.GetString(SessionConstants.UserId);

            //    if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            //    {
            //        model.EmployID = userIdStr;



            //        TempData["SuccessMessage"] = "Thank you for your feedback!";
            //        return RedirectToAction("Submit");
            //    }
            //    else
            //    {

            //        TempData["ErrorMessage"] = "User session has expired. Please log in again.";
            //        return RedirectToAction("Login", "Account"); // or wherever appropriate
            //    }
            //}

            return this.RedirectToAction("Submit");
        }

    }
}
