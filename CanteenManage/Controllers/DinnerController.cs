using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class DinnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
