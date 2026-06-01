using System.Net;
using ClinicApi.App.Common;

namespace ClinicApi.Presentation.Middleware
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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string message;

            switch (ex)
            {
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case InvalidOperationException:
                    status = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred";
                    break;
            }

            var response = new ErrorResponse
            {
                Message = message,
                StatusCode = (int)status,
                Details = ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}