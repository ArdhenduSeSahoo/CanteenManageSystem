using CanteenManage.Services;

namespace CanteenManage.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppConfigProvider _appConfigProvider;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                _logger.LogError("An error occurred while processing the request. From ErrorHandlerMiddleware---" + ex);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { status = "Some error Found. Please login again." });
            }

        }
    }
}