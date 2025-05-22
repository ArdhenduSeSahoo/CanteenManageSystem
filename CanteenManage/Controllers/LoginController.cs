using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CanteenManage.Controllers
{

    public class LoginController : Controller
    {
        private readonly AppConfigProvider appConfigProvider;
        private readonly LoginService loginService;
        public LoginController(AppConfigProvider appConfig, LoginService loginService)
        {
            this.appConfigProvider = appConfig;
            this.loginService = loginService;
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> testlog(string? empid, string? empname)
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }

            if (string.IsNullOrEmpty(empid))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid))
            {
                var userFound = await loginService.IsValidEmployee(empid, empname);
                if (userFound != null)
                {
                    if (userFound.EmployTypeId == 3)
                    {
                        return setEmployeeSessionAndRedirect(userFound.Name, userFound.Id);
                    }
                    else if (userFound.EmployTypeId == 4)
                    {
                        var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, userFound.Name),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
                        var jwttokens = loginService.GenerateJSONWebToken(claims);
                        SetJWTCookie(jwttokens);
                        //HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                        //HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                        return this.RedirectToAction(actionName: "ChoseModeOfUse", controllerName: "Login", new { empid = userFound.Id, empname = userFound.Name });
                    }

                }
                else
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Error");
                }
            }

            return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }
        [AllowAnonymous]
        public async Task<IActionResult> emp(string? empid, string? empname)
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }

            var emp = empid;
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname))
            {
                var userFound = await loginService.IsValidEmployee(empid, empname);
                if (userFound != null)
                {
                    if (userFound.EmployTypeId == 3)
                    {
                        return setEmployeeSessionAndRedirect(userFound.Name, userFound.Id);
                    }
                    else if (userFound.EmployTypeId == 4)
                    {
                        var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, userFound.Name),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
                        var jwttokens = loginService.GenerateJSONWebToken(claims);
                        SetJWTCookie(jwttokens);
                        //HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                        //HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                        return this.RedirectToAction(actionName: "ChoseModeOfUse", controllerName: "Login", new { empid = userFound.Id, empname = userFound.Name });
                    }

                }
                else
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Error");
                }
            }

            return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }

        [Authorize(Roles = "Employee")]
        public IActionResult ChoseModeOfUse(string empid, string empname)
        {
            ViewBag.empid = empid;
            ViewBag.empname = empname;
            return View();
        }

        [Authorize]
        public IActionResult LoginAsEmployee(string empid, string empname)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname))
            {
                try
                {
                    return setEmployeeSessionAndRedirect(empname, int.Parse(empid));
                }
                catch (Exception ex)
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Error");
                }

            }
            else
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }
        [Authorize]
        public IActionResult LoginAsCanteenMember(string empid, string empname)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname))
            {
                try
                {
                    return setCommitmemberSessionAndRedirect(empname, int.Parse(empid));
                }
                catch (Exception ex)
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Error");
                }
            }
            else
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }

            return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                var userFound = loginService.IsValidUser(userId, password);
                //canteenManageContext.Employes.Where(e => e.EmployID == userId).FirstOrDefault();
                if (userFound == null)
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Login");
                }

                if (userFound.EmployTypeId == 2)
                {
                    var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, userFound.Name),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleCanteenEmploy) // Add user role
                        };
                    var jwttokens = loginService.GenerateJSONWebToken(claims);
                    SetJWTCookie(jwttokens);

                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                    return this.RedirectToAction(actionName: "Index", controllerName: "CanteenEmploy");
                }
                //if (userFound.EmployTypeId == 4)
                //{
                //    var claims = new List<Claim>
                //        {
                //            new Claim(JwtRegisteredClaimNames.Sub, ""),
                //            new Claim(ClaimTypes.Name, "Ardhendu"),
                //            new Claim(ClaimTypes.Role, CustomDataConstants.RoleCommitteeMemberOrEmployee) // Add user role
                //        };
                //    var jwttokens = loginService.GenerateJSONWebToken(claims);
                //    SetJWTCookie(jwttokens);
                //    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                //    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                //    return this.RedirectToAction(actionName: "CMDashboard", controllerName: "CommitteeMember");
                //}
            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }

        private RedirectToActionResult setEmployeeSessionAndRedirect(string Ename, int Eid)
        {
            var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, Ename),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
            var jwttokens = loginService.GenerateJSONWebToken(claims);
            SetJWTCookie(jwttokens);

            HttpContext.Session.SetString(SessionConstants.UserId, Eid.ToString());
            HttpContext.Session.SetString(SessionConstants.UserName, Ename.ToString());
            return this.RedirectToAction(actionName: "Index", controllerName: "EmployDashboard");
        }

        private RedirectToActionResult setCommitmemberSessionAndRedirect(string Ename, int Eid)
        {
            var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, Ename),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleCommitteeMember) // Add user role
                        };
            var jwttokens = loginService.GenerateJSONWebToken(claims);
            SetJWTCookie(jwttokens);

            HttpContext.Session.SetString(SessionConstants.UserId, Eid.ToString());
            HttpContext.Session.SetString(SessionConstants.UserName, Ename.ToString());
            return this.RedirectToAction(actionName: "CMDashboard", controllerName: "CommitteeMember");
        }

        public IActionResult LoginSignOut()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }

        public IActionResult LoginSignOutEmployee()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }



        private void SetJWTCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Uncomment this line if your application is running over HTTPS
                Expires = DateTime.UtcNow.AddHours(3),
            };
            Response.Cookies.Append(CustomDataConstants.jwtTokencookieName, token, cookieOptions);
        }
    }
}
