using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class ErrorController : Controller
    {
        private readonly AppConfigProvider appConfigProvider;

        public ErrorController(AppConfigProvider appConfigProvider)
        {
            this.appConfigProvider = appConfigProvider;
        }

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
            if (appConfigProvider.IsDevelopmentEnv())
            {
                ViewBag.redireURL = "/login/";
            }
            else
            {
                ViewBag.redireURL = appConfigProvider.GetLogOutURL();
            }
            return View();
        }
    }
}
