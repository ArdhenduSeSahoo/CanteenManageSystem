using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CanteenManage.Controllers
{
    public class ErrorController : Controller
    {
        private readonly AppConfigProvider appConfigProvider;

        public ErrorController(AppConfigProvider appConfigProvider)
        {
            this.appConfigProvider = appConfigProvider;
        }

        public IActionResult Index(string? jasowerukasj)
        {
            Exception? exception = null;
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);
                if (!string.IsNullOrWhiteSpace(jasowerukasj))
                {
                    exception = JsonConvert.DeserializeObject<Exception>(jasowerukasj);
                }

            }
            catch (Exception ex)
            {

            }
            ErrorViewDataModels errorViewData = new ErrorViewDataModels();
            if (appConfigProvider.IsDevelopmentEnv())
            {
                errorViewData.RedirectURL = "/login/";
                errorViewData.RedirectLinkName = "Go to Login";
                errorViewData.Error = exception?.StackTrace ?? "";
            }
            else
            {
                errorViewData.RedirectURL = appConfigProvider.GetLogOutURL() ?? "https://econnect.esspl.com/";
                errorViewData.RedirectLinkName = "Esspl E-Connect";
                errorViewData.Error = exception?.Message ?? "";
            }
            return View(errorViewData);
        }
    }
}
