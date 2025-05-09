using CanteenManage.Repo.Contexts;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class EmployDashboardController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        public EmployDashboardController()
        {

        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString(SessionConstants.UserId) is null)
            {
                return RedirectToAction("Login", "Index");
            }

            return View();
        }
    }
}
