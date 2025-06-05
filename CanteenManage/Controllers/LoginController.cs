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
using Newtonsoft.Json;

namespace CanteenManage.Controllers
{

    public class LoginController : Controller
    {
        private readonly AppConfigProvider appConfigProvider;
        private readonly LoginService loginService;
        private ILogger logger;
        public LoginController(AppConfigProvider appConfig, LoginService loginService, ILogger<LoginController> logger)
        {
            this.appConfigProvider = appConfig;
            this.loginService = loginService;
            this.logger = logger;
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

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> PortalLogin(string? portal_token)
        {
            logger.LogError("PortalLogin accessed");
            var headerdata = HttpContext.Request.Headers.Select(hd => new { key = hd.Key, val = hd.Value }).ToList();
            logger.LogError(JsonConvert.SerializeObject(HttpContext.Request.Headers));
            return await loginUserAsync("SD1265", "Ardhendu", portal_token);
        }

        [AllowAnonymous]
        public async Task<IResult> PortalLogOut()
        {
            logger.LogError("PortalLogOut accessed");
            var headerdata = HttpContext.Request.Headers.Select(hd => new { key = hd.Key, val = hd.Value }).ToList();
            logger.LogError(JsonConvert.SerializeObject(HttpContext.Request.Headers));

            return Results.Ok(new { Status = "Request accepted. User will sign out." });
        }

        [AllowAnonymous]
        public async Task<IActionResult> testlog(string? empid, string? empname)
        {

            return await loginUserAsync(empid, empname);
        }
        [AllowAnonymous]
        public async Task<IActionResult> emp(string? empid, string? empname)
        {
            return await loginUserAsync(empid, empname);
            //try
            //{
            //    HttpContext.Session.Clear();
            //    HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);

            //}
            //catch (Exception ex)
            //{

            //}

            //var emp = empid;
            //if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname))
            //{
            //    return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            //}
            //else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname))
            //{
            //    var userFound = await loginService.IsValidEmployee(empid, empname);
            //    if (userFound != null)
            //    {
            //        if (userFound.EmployTypeId == 3)
            //        {
            //            return setEmployeeSessionAndRedirect(userFound.Name, userFound.Id, userFound.EmployID);
            //        }
            //        else if (userFound.EmployTypeId == 4)
            //        {
            //            var claims = new List<Claim>
            //            {
            //                //new Claim(JwtRegisteredClaimNames.Sub, ""),
            //                new Claim(ClaimTypes.Name, userFound.Name),
            //                new Claim(ClaimTypes.Role, CustomDataConstants.RoleEmployee) // Add user role
            //            };
            //            var jwttokens = loginService.GenerateJSONWebToken(claims);
            //            SetJWTCookie(jwttokens);
            //            //HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
            //            //HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
            //            return this.RedirectToAction(actionName: "ChoseModeOfUse", controllerName: "Login", new { eid = userFound.Id, empid = userFound.EmployID, empname = userFound.Name });
            //        }

            //    }
            //    else
            //    {
            //        return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            //    }
            //}

            //return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }

        [AllowAnonymous]
        public async Task<IActionResult> empt(string? empid, string? empname)
        {
            return await loginUserAsync(empid, empname);
        }
        public async Task<RedirectToActionResult> loginUserAsync(string? empid, string? empname, string? portal_token = "")
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
                        return setEmployeeSessionAndRedirect(userFound.Name, userFound.Id, userFound.EmployID, portal_token);
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
                        HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                        HttpContext.Session.SetString(SessionConstants.UserEmpId, userFound.EmployID.ToString());
                        HttpContext.Session.SetString(SessionConstants.UserName, userFound.Name.ToString());
                        HttpContext.Session.SetString(SessionConstants.EconnectToken, portal_token ?? "");
                        return this.RedirectToAction(actionName: "ChoseModeOfUse", controllerName: "Login", new { eid = userFound.Id, empid = userFound.EmployID, empname = userFound.Name });
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
        public IActionResult ChoseModeOfUse(string empid, string empname, string eid)
        {
            ViewBag.empid = empid;
            ViewBag.eid = eid;
            ViewBag.empname = empname;
            return View();
        }

        //[Authorize]
        public IActionResult LoginAsEmployee(string empid, string empname, string eid)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname) || string.IsNullOrEmpty(eid))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname) || !string.IsNullOrEmpty(eid))
            {
                try
                {
                    return setEmployeeSessionAndRedirect(empname, int.Parse(eid), empid, "");
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
        //[Authorize]
        public IActionResult LoginAsCanteenMember(string empid, string empname, string eid)
        {
            if (string.IsNullOrEmpty(empid) || string.IsNullOrEmpty(empname) || string.IsNullOrEmpty(eid))
            {
                return this.RedirectToAction(actionName: "Index", controllerName: "Error");
            }
            else if (!string.IsNullOrEmpty(empid) || !string.IsNullOrEmpty(empname) || !string.IsNullOrEmpty(eid))
            {
                try
                {
                    return setCommitmemberSessionAndRedirect(empname, int.Parse(eid), empid);
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
            //HttpContext.Session.SetString("UserName", Request.Form["username"]);
            //string userId = "";
            //string userPassword = "";

            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(username) && string.IsNullOrEmpty(userPassword))
                {
                    return await loginUserAsync(userId, username);
                }
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userPassword))
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
                    var jwttokens = loginService.GenerateJSONWebToken(claims);
                    SetJWTCookie(jwttokens);

                    HttpContext.Session.SetString(SessionConstants.UserId, userFound.Id.ToString());
                    HttpContext.Session.SetString(SessionConstants.UserEmpId, userFound.EmployID.ToString());
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

        private RedirectToActionResult setEmployeeSessionAndRedirect(string Ename, int Eid, string empId, string? econnecttoken)
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
            HttpContext.Session.SetString(SessionConstants.UserEmpId, empId.ToString());
            if (!string.IsNullOrWhiteSpace(econnecttoken))
            {
                HttpContext.Session.SetString(SessionConstants.EconnectToken, econnecttoken ?? "");
            }

            return this.RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }

        private RedirectToActionResult setCommitmemberSessionAndRedirect(string Ename, int Eid, string empId)
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
            HttpContext.Session.SetString(SessionConstants.UserEmpId, empId.ToString());
            return this.RedirectToAction(actionName: "CMDashboard", controllerName: "CommitteeMember");
        }

        public IActionResult LoginSignOut()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Response.Cookies.Delete(CustomDataConstants.jwtTokencookieName);
                //HttpClient httpClient = new HttpClient();
                //httpClient.BaseAddress = new Uri("http://192.168.0.82/");
                //httpClient.DefaultRequestHeaders.Accept.Clear();
                //var responsess = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")).Result;

            }
            catch (Exception ex)
            {

            }
            //return this.RedirectToAction(actionName: "Index", controllerName: "Login");
            return Redirect("http://192.168.0.82/");
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
            return Redirect("http://192.168.0.82/");
            //return this.RedirectToAction(actionName: "Index", controllerName: "Error");
        }



        private void SetJWTCookie(string token)
        {
            HttpContext.Session.SetString(CustomDataConstants.jwtTokencookieName, token);

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
