using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class EmployListOfFoodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
