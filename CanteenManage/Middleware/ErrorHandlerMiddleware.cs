using CanteenManage.Services;

namespace CanteenManage.Middleware
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        //private readonly AppConfigProvider _appConfigProvider;
        private readonly ILogger<ErrorHandlerMiddleWare> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ErrorHandlerMiddleWare(RequestDelegate next, ILogger<ErrorHandlerMiddleWare> logger, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
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
                if (_webHostEnvironment.IsDevelopment())
                {
                    _logger.LogError("An error occurred while processing the request. From ErrorHandlerMiddleWare---" + ex.Message);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { status = "Some error Found." + ex.StackTrace });
                    return;
                }
                else
                {
                    _logger.LogError("An error occurred while processing the request. From ErrorHandlerMiddleWare---" + ex.Message);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { status = "Error Page--" + ex.StackTrace });
                    return;
                }
                _logger.LogError("An error occurred while processing the request. From ErrorHandlerMiddleWare---" + ex);
                //context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { status = "Some error Found. Please login again." });
            }

        }
    }
}