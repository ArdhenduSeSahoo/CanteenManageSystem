using CanteenManage.Models;
using CanteenManage.Utility;

namespace CanteenManage.Middleware
{
    public class SessionValidateMiddleWare
    {
        private readonly RequestDelegate _next;
        public SessionValidateMiddleWare(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            SessionDataModel sessionData = new SessionDataModel();
            var requestPath = (context.Request.Path.Value ?? "").Trim().ToLower();

            var empid = context.Session.GetString(SessionConstants.UserId);
            var empEid = context.Session.GetString(SessionConstants.UserEmpId);
            if (requestPath == "/")
            {
                //context.Response.StatusCode = 401; // Unauthorized
                context.Response.Redirect("/login/index");
                return;
            }
            else if (IsAllowedURL(context))
            {
                //context.Response.Redirect("/Login/emp");
                //return;
                await _next(context);
                return;
            }
            else if (string.IsNullOrEmpty(empEid) || string.IsNullOrEmpty(empEid))
            {
                context.Response.Redirect("/Error"); return;
            }
            else
            {
                await _next(context);
            }
        }
        public bool IsAllowedURL(HttpContext context)
        {
            return AllowedEndPoints.AllowedURL_List.Contains(context.Request.Path.Value?.ToLower() ?? "");
        }
    }
}
