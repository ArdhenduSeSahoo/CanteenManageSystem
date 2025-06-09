using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
//using NuGet.Common;

namespace CanteenManage.Controllers
{

    public class LoginController : Controller
    {
        private readonly AppConfigProvider appConfigProvider;
        private readonly LoginService loginService;
        private ILogger logger;
        private readonly SessionManager sessionManager;
        private string tempEmpId = "";
        private string tempPortalToken = "";
        private int HourOfSession = 3;
        public LoginController(AppConfigProvider appConfig, LoginService loginService, ILogger<LoginController> logger, SessionManager sessionManager)
        {
            this.appConfigProvider = appConfig;
            this.loginService = loginService;
            this.logger = logger;
            this.sessionManager = sessionManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            //logger.LogInformation("Login accessed");
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            }
            catch (Exception ex)
            {

            }
            ViewBag.IsDevEnv = appConfigProvider.IsDevelopmentEnv();
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> PortalLogin(string? portal_token)
        {
            var tokesss = "";
            // portal_token = tokesss.Trim();
            //var handler = new JwtSecurityTokenHandler();
            //var jwtToken = handler.ReadJwtToken(tokesss);
            //var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            //var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            //var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            //var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            return await loginUserAsync("", "", portal_token);
        }

        [AllowAnonymous]
        public async Task<IResult> PortalLogOut(string? portal_token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(portal_token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    await loginService.LogOutUpdateEmployee(userId);
                    return Results.Ok(new { Status = "Request accepted. User will sign out." });
                }
                else
                {
                    return Results.BadRequest(new { Status = "Some error happened--" });
                }
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Status = "Some error happened--" + ex.Message });
            }
        }
        public async Task<RedirectToActionResult> loginUserAsync(string? empid, string? empname, string portal_token = "")
        {
            string empEmail = "";
            string expv = "";
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

                var tokesss = "";
                if (!string.IsNullOrWhiteSpace(portal_token))
                {
                    tokesss = portal_token;
                }
                //tokesss = "";
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokesss);
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    empEmail = email;
                }
                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    empname = name;
                }
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    empid = userId;
                }
                //var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                //if (!string.IsNullOrWhiteSpace(exp))
                //{
                //    expv = exp;
                //}
                //double ticks = double.Parse(exp);
                //TimeSpan time = TimeSpan.FromMilliseconds(ticks);
                //DateTime dateTime = DateTime.Now.Date + time;
            }
            catch (Exception ex)
            {
                logger.LogError("Error in reading JWT token: " + ex.Message);
                logger.LogError($"Token--: {portal_token}");
                logger.LogError(ex.StackTrace);
                //return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }


            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname))
            {
                var userFound = await loginService.GetOrAddEmployee(empid, empname, empEmail);
                if (userFound != null)
                {
                    if (userFound.EmployTypeId == (int)EmployTypeEnum.Employee)
                    {
                        var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Email, userFound.Email??""),
                            new Claim(ClaimTypes.Name, userFound.Name),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
                        return await setEmployeeSessionAndRedirect(userFound.Name, userFound.Id, userFound.EmployID, portal_token, claims, DateTime.Now.AddHours(HourOfSession));
                    }
                    else if (userFound.EmployTypeId == (int)EmployTypeEnum.Committee_Members)
                    {
                        var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Name, userFound.Name),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
                        var jwttokens = loginService.GenerateJSONWebToken(claims, DateTime.Now.AddHours(4));
                        SetJWTCookie(jwttokens, userFound.EmployID, portal_token, ((int)EmployTypeEnum.Committee_Members).ToString());

                        HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                        HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                        return this.RedirectToAction(actionName: "ChoseModeOfUse", controllerName: "Login", new { eid = userFound.Id, empid = userFound.EmployID, empname = userFound.Name, empmail = userFound.Email ?? "" });
                    }
                    else
                    {
                        return this.RedirectToAction(actionName: "Index", controllerName: "Error");
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
        public IActionResult ChoseModeOfUse(string empid, string empname, string eid, string empmail)
        {
            ViewBag.empid = empid;
            ViewBag.eid = eid;
            ViewBag.empname = empname;
            ViewBag.empmail = empmail;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> LoginAsEmployee(string empid, string empname, string eid, string empmail)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname) || string.IsNullOrEmpty(eid))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname) || !string.IsNullOrEmpty(eid))
            {
                try
                {
                    empmail = string.IsNullOrWhiteSpace(empmail) ? "" : empmail;
                    empname = string.IsNullOrWhiteSpace(empname) ? "" : empname;
                    var claims = new List<Claim>
                        {
                            //new Claim(JwtRegisteredClaimNames.Sub, ""),
                            new Claim(ClaimTypes.Email, empmail),
                            new Claim(ClaimTypes.Name, empname),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
                        };
                    return await setEmployeeSessionAndRedirect(empname, int.Parse(eid), empid, "", claims, DateTime.Now.AddHours(3));
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
        public IActionResult LoginAsCommitMember(string empid, string empname, string eid, string empmail)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname) || string.IsNullOrEmpty(eid))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname) || !string.IsNullOrEmpty(eid))
            {
                try
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, empmail),
                            new Claim(ClaimTypes.Name, empname),
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleCommitteeMember) // Add user role
                        };
                    return setCommitmemberSessionAndRedirect(empname, int.Parse(eid), empid, "", claims, DateTime.Now.AddHours(3));
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
        public async Task<IActionResult> LoginUser(string userId, string username, string userPassword)//IFormCollection formcollect
        {

            try
            {
                if (appConfigProvider.IsDevelopmentEnv())
                {
                    if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(userPassword))
                    {
                        return await loginUserAsync(userId, username);
                    }
                }
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userPassword))
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Login");
                }
                var userFound = loginService.IsValidUser(userId, userPassword);
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
                            new Claim(ClaimTypes.Role, CustomDataConstants.RoleCanteenEmployee) // Add user role
                        };
                    var jwttokens = loginService.GenerateJSONWebToken(claims, DateTime.Now.AddHours(8));
                    SetJWTCookie(jwttokens, userId, "", ((int)EmployTypeEnum.CanteenStaf).ToString());

                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserEmpId, userFound.EmployID.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                    return this.RedirectToAction(actionName: "Index", controllerName: "CanteenEmploy");
                }
            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");
        }
        /// <summary>
        /// Eid is PK, empID is company ID, Ename is employee name
        /// econnecttoken is null here bcz it is set previously in loginUserAsync function
        /// </summary>
        /// <returns></returns>
        private async Task<RedirectToActionResult> setEmployeeSessionAndRedirect(string Ename, int Eid, string empId, string econnecttoken, List<Claim> claims, DateTime expireDatetime)
        {

            var jwttokens = loginService.GenerateJSONWebToken(claims, expireDatetime);
            SetJWTCookie(jwttokens, empId, econnecttoken, ((int)EmployTypeEnum.Employee).ToString());

            HttpContext.Session.SetString(SessionConstants.UserId, Eid.ToString());
            HttpContext.Session.SetString(SessionConstants.UserName, Ename.ToString());

            await loginService.LoginUpdateEmployee(empId);

            return this.RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }
        /// <summary>
        /// econnecttoken is null here bcz it is set previously in loginUserAsync function
        /// </summary>
        private RedirectToActionResult setCommitmemberSessionAndRedirect(string Ename, int Eid, string empId, string econnecttoken, List<Claim> claims, DateTime expireDatetime)
        {

            var jwttokens = loginService.GenerateJSONWebToken(claims, expireDatetime);
            SetJWTCookie(jwttokens, empId, econnecttoken, ((int)EmployTypeEnum.Committee_Members).ToString());

            HttpContext.Session.SetString(SessionConstants.UserId, Eid.ToString());
            HttpContext.Session.SetString(SessionConstants.UserName, Ename.ToString());
            return this.RedirectToAction(actionName: "CMDashboard", controllerName: "CommitteeMember");
        }

        public IActionResult LoginSignOut()
        {
            try
            {
                HttpContext.Session.Clear();
                //HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

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
                //HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);
                if (appConfigProvider.IsDevelopmentEnv())
                {
                    return this.RedirectToAction(actionName: "Index", controllerName: "Login");
                }
                else
                {
                    var logouturl = appConfigProvider.GetLogOutURL();
                    return Redirect(logouturl);
                }
            }
            catch (Exception ex)
            {

            }
            return this.RedirectToAction(actionName: "Index", controllerName: "Login");

        }

        private void SetJWTCookie(string AppToken, string userEmpId, string PortalToken, string userType)
        {
            HttpContext.Session.SetString(SessionConstants.AppToken, AppToken);
            HttpContext.Session.SetString(SessionConstants.UserEmpId, userEmpId);
            HttpContext.Session.SetString(SessionConstants.UserType, userType);
            if (!string.IsNullOrWhiteSpace(PortalToken))
            {
                HttpContext.Session.SetString(SessionConstants.EconnectToken, PortalToken);
            }
            SessionUser sessionUser = new SessionUser
            {
                UserEmpID = userEmpId,
                Portal_Token = PortalToken,
                isLogin = true
            };
            sessionManager.AddOrLoginSessionUser(userEmpId, PortalToken);
            //var cookieOptions = new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true, // Uncomment this line if your application is running over HTTPS
            //    Expires = DateTime.UtcNow.AddHours(3),
            //};
            //Response.Cookies.Append(CustomDataConstants.jwtTokencookieName, token, cookieOptions);
        }
    }
}
