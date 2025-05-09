using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers.CanteenControllers
{
    public class EmployListOfFoodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
