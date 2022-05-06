using System.Net;

namespace RoomBooking.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(httpContext, ex);
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(exception.Message, exception);
        }
    }
}
