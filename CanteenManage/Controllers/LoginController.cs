using CanteenManage.CanteenRepository.Contexts;
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
                //userId = "EMP003";password = "sadf";
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Login");
                }
                var userFound = canteenManageContext.Employes.Where(e => e.EmployID == userId).FirstOrDefault();
                if (userFound == null)
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Login");
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
                if (userFound.EmployTypeId == 4)
                {
                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                    return this.RedirectToAction(actionName: "CMDashboard", controllerName: "CommitteeMember");
                }
            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }
    }
}
