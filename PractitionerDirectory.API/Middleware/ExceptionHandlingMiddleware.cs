using PractitionerDirectory.Services.CustomExceptions;
using System.Net;
using System.Text.Json;

namespace PractitionerDirectory.API.Middleware
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошло необработанное исключение: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            string message;


            switch (exception)
            {
                case DuplicatePractitionerException duplicateEx:
                    statusCode = HttpStatusCode.Conflict;
                    message = duplicateEx.Message;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "На сервере произошла внутренняя ошибка. Попробуйте позже.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                StatusDescription = statusCode.ToString(),
                Error = message,
                Timestamp = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(payload);
        }
    }
}
