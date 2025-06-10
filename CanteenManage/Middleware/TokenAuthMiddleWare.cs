// Ignore Spelling: Auth

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using CanteenManage.Middleware;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace CanteenManage.CanteenMiddleWare
{
    public class TokenAuthMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly SessionManager _sessionManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppConfigProvider _appConfigProvider;
        private readonly ILogger<TokenAuthMiddleWare> _logger;
        public TokenAuthMiddleWare(RequestDelegate next, SessionManager sessionManager, IHttpClientFactory httpClientFactory, ILogger<TokenAuthMiddleWare> logger, AppConfigProvider appConfigProvider)
        {
            _next = next;
            _sessionManager = sessionManager;
            _httpClientFactory = httpClientFactory;
            _appConfigProvider = appConfigProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            //var tok = context.Request.Cookies[CustomDataConstants.jwtTokencookieName];
            string? App_token = context.Session.GetString(SessionConstants.AppToken);
            string? EConnect_token = context.Session.GetString(SessionConstants.EconnectToken);
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
            else if (string.IsNullOrEmpty(App_token))
            {
                //context.Response.StatusCode = 401; // Unauthorized
                context.Response.Redirect("/Error");
                return;
            }
            else
            {
                try
                {
                    var usertype_int = int.Parse(userType ?? "3");
                    if (usertype_int == (int)EmployTypeEnum.Employee)
                    {
                        if (!string.IsNullOrWhiteSpace(userEmpID)
                            && _sessionManager.IsUserLoggedIn(userEmpID)
                            )
                        {
                            //string responsbody = "";
                            //_logger.LogError($"Econnect token--" + EConnect_token);
                            if (!_appConfigProvider.IsDevelopmentEnv())
                            {
                                try
                                {
                                    var httpClient = _httpClientFactory.CreateClient(CustomDataConstants.PortalAuthValidater);
                                    httpClient.Timeout = new TimeSpan(0, 0, 50);
                                    httpClient.DefaultRequestHeaders.Clear();
                                    httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + EConnect_token);
                                    var responsdata = await httpClient.PatchAsync(_appConfigProvider.GetPortalAuthValidaTorEndpoint(), null);
                                    //_logger.LogError($"Calling e connect auth path--{_appConfigProvider.GetPortalAuthValidaTorEndpoint()} token-{EConnect_token}");
                                    responsdata.EnsureSuccessStatusCode();
                                    //responsbody = await responsdata.Content.ReadAsStringAsync();
                                    //_logger.LogError(responsbody);

                                    context.Request.Headers["Authorization"] = "Bearer " + App_token;
                                    await _next(context);
                                    //Console.WriteLine(responsdata.ToString());
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"Token validation failed: {ex.Message}---- for user id--{userEmpID}-----E connect token-----{EConnect_token}");
                                    context.Response.Redirect("/Error");
                                    return;
                                }
                            }
                            else
                            {
                                context.Request.Headers["Authorization"] = "Bearer " + App_token;
                                await _next(context);
                            }
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
                        context.Request.Headers["Authorization"] = "Bearer " + App_token;
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
                    _logger.LogError($"TokenAuthMiddleware failed: {ex.Message}---- for user id--{userEmpID}");
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
