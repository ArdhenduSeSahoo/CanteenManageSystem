using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }
            return View();
        }
    }
}
