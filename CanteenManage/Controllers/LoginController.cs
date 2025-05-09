using CanteenManage.Repo.Contexts;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManage.Controllers
{
    public class LoginController : Controller
    {
        private readonly CanteenManageDBContext canteenManageContext;
        public LoginController(CanteenManageDBContext canteenManageContext)
        {
            this.canteenManageContext = canteenManageContext;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(IFormCollection formcollect)
        {
            //HttpContext.Session.SetString("UserName", Request.Form["username"]);
            string userId = "", password = "";
            //string userId = "SD1265", password = "sadf";
            try
            {
                userId = formcollect["userId"].ToString();
                password = formcollect["userPassword"].ToString();
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    return View();
                }
                var userFound = canteenManageContext.Employes.Where(e => e.EmployID == userId).FirstOrDefault();
                if (userFound == null)
                {
                    return View();
                }
                if (userFound.EmployTypeId == 3)
                {
                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                    return this.RedirectToAction(actionName: "Index", controllerName: "EmployDashboard");
                }
                if (userFound.EmployTypeId == 2)
                {
                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                    return this.RedirectToAction(actionName: "Index", controllerName: "CanteenEmploy");
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }
    }
}
