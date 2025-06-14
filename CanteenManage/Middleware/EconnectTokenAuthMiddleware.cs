﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CanteenManage.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class EconnectTokenAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public EconnectTokenAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class EconnectTokenAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseEconnectTokenAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EconnectTokenAuthMiddleware>();
        }
    }
}
