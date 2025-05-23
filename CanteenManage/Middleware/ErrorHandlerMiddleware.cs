using CanteenManage.Services;

namespace CanteenManage.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppConfigProvider _appConfigProvider;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            //_appConfigProvider = appConfigProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //if (_appConfigProvider.IsDevelopment())
                //{
                //    context.Response.StatusCode = StatusCodes.Status404NotFound;
                //    await context.Response.WriteAsJsonAsync(new { status = "Some error Found." + ex.Message });
                //}

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { status = "Some error Found. Please login again." });
            }

        }
    }
}