// Ignore Spelling: Auth

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CanteenManage.Utility;
using Microsoft.IdentityModel.Tokens;

namespace CanteenManage.CanteenMiddleWare
{
    public class TokenAuthMiddleWare
    {
        private readonly RequestDelegate _next;
        public TokenAuthMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var tok = context.Request.Cookies[CustomDataConstants.jwtTokencookieName];
            var requestPath = (context.Request.Path.Value ?? "").Trim().ToLower();
            if (!string.IsNullOrEmpty(requestPath))
            {
                if (requestPath == "/")
                {
                    //context.Response.StatusCode = 401; // Unauthorized
                    context.Response.Redirect("/login/index");
                    return;
                }
                else if (
                    (requestPath.StartsWith("/login/index")
                    || requestPath.StartsWith("/login/emp")
                    || requestPath.StartsWith("/login/testlog")
                    || requestPath.StartsWith("/error")
                    || requestPath.StartsWith(("/Login/LoginUser").ToLower())
                    && !requestPath.Contains("/login/chosemodeofuse")
                    )
                    //&&
                    //(
                    //!requestPath.Contains("/login/chosemodeofuse")
                    //|| !requestPath.StartsWith("/login/loginasemployee")
                    //|| !requestPath.StartsWith("/login/loginascanteenmember")
                    //)
                    )
                {
                    //context.Response.Redirect("/Login/emp");
                    //return;
                    await _next(context);
                }
                //else if (requestPath.StartsWith("/Login/emp"))
                //{
                //    //context.Response.Redirect("/Login/emp");
                //    //return;
                //    await _next(context);
                //}
                //else if (string.IsNullOrEmpty(tok) && requestPath.StartsWith(("/Login/LoginUser").ToLower()))
                //{
                //    await _next(context);
                //}
                else if (string.IsNullOrEmpty(tok))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }
                else
                {
                    //if (string.IsNullOrEmpty(tok))
                    {
                        context.Request.Headers["Authorization"] = "Bearer " + tok;
                    }
                    await _next(context);
                }
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
            //if (string.IsNullOrEmpty(tok) && context.Request.Path == "/")
            //{
            //    //context.Response.StatusCode = 401; // Unauthorized
            //    context.Response.Redirect("/Login/Index");
            //    return;
            //}
            //else if ((context.Request.Path.Value ?? "").Contains("/Login/Index"))
            //{
            //    //context.Response.Redirect("/Login/emp");
            //    //return;
            //    await _next(context);
            //}
            //else if ((context.Request.Path.Value ?? "").Contains("/Login/emp"))
            //{
            //    //context.Response.Redirect("/Login/emp");
            //    //return;
            //    await _next(context);
            //}
            //else if (string.IsNullOrEmpty(tok) && (context.Request.Path.Value ?? "").Contains("/Login/emp"))
            //{
            //    context.Response.Redirect("/Login/emp");
            //    return;
            //}
            //else if (!string.IsNullOrEmpty(tok) && (context.Request.Path.Value ?? "").Contains("/Login/emp"))
            //{
            //    context.Request.Headers["Authorization"] = "Bearer " + context.Request.Cookies[CustomDataConstants.cookieName];
            //}
            //else if (string.IsNullOrEmpty(tok) && !(context.Request.Path.Value ?? "").Contains("/Login/Index"))
            //{
            //    context.Response.StatusCode = 401; // Unauthorized
            //    return;
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(tok))
            //    {
            //        context.Request.Headers["Authorization"] = "Bearer " + tok;
            //    }

            //}
            //var token = ""; //context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var validationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //    try
            //    {
            //        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            //    }
            //    catch (Exception)
            //    {
            //        context.Response.StatusCode = 401; // Unauthorized
            //        return;
            //    }
            //}


        }
    }
}
