// Ignore Spelling: Auth

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CanteenManage.Middleware;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.IdentityModel.Tokens;

namespace CanteenManage.CanteenMiddleWare
{
    public class TokenAuthMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly SessionManager _sessionManager;
        public TokenAuthMiddleWare(RequestDelegate next, SessionManager sessionManager)
        {
            _next = next;
            _sessionManager = sessionManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            //var tok = context.Request.Cookies[CustomDataConstants.jwtTokencookieName];
            var tok = context.Session.GetString(SessionConstants.AppToken);
            var userEmpID = context.Session.GetString(SessionConstants.UserEmpId);
            var userType = context.Session.GetString(SessionConstants.UserType);

            var requestPath = (context.Request.Path.Value ?? "").Trim().ToLower();

            if (requestPath == "/")
            {
                //context.Response.StatusCode = 401; // Unauthorized
                //context.Response.Redirect("/login/index");
                await _next(context);
                return;
            }
            else if (IsAllowedURL(context))
            {
                //context.Response.Redirect("/Login/emp");
                //return;
                await _next(context);
            }
            else if (string.IsNullOrEmpty(tok))
            {
                //context.Response.StatusCode = 401; // Unauthorized
                context.Response.Redirect("/Error");
                return;
            }
            else
            {
                try
                {
                    var usertype_int = int.Parse(userType);
                    if (usertype_int == (int)EmployTypeEnum.Employee)
                    {
                        if (!string.IsNullOrWhiteSpace(userEmpID)
                            && _sessionManager.IsUserLoggedIn(userEmpID)
                            )
                        {
                            context.Request.Headers["Authorization"] = "Bearer " + tok;
                            await _next(context);
                        }
                        else
                        {
                            //context.Response.StatusCode = 401; // Unauthorized
                            context.Response.Redirect("/Error");
                            return;
                        }
                    }
                    else if (usertype_int == (int)EmployTypeEnum.Committee_Members || usertype_int == (int)EmployTypeEnum.CanteenStaf
                            )//check for member login and canteen employ login
                    {
                        context.Request.Headers["Authorization"] = "Bearer " + tok;
                        await _next(context);
                    }
                    else
                    {
                        //context.Response.StatusCode = 401; // Unauthorized
                        context.Response.Redirect("/Error");
                        return;
                    }
                }
                catch (Exception ex)
                {

                    context.Response.Redirect("/Error");
                    return;
                }
            }

        }

        public bool IsAllowedURL(HttpContext context)
        {
            return AllowedEndPoints.AllowedURL_List.Contains(context.Request.Path.Value?.ToLower() ?? "");
        }
    }
}
