using System.Text.Json;
using FluentValidation;

namespace MisFinanzas.API.Middleware
{
    /// <summary>
    /// Middleware global que captura las excepciones no controladas y las traduce
    /// a una respuesta HTTP con el código correcto y un formato de error estándar.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // deja pasar la petición al resto del pipeline
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;

            // Por defecto: error no controlado -> 500
            var statusCode = StatusCodes.Status500InternalServerError;
            var errorCode = "INTERNAL_ERROR";
            var message = "Ocurrió un error inesperado.";
            var details = new List<string>();

            switch (ex)
            {
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorCode = "VALIDATION_ERROR";
                    message = "Uno o más campos no son válidos.";
                    details = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                    break;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorCode = "NOT_FOUND";
                    message = ex.Message;
                    break;

                case InvalidOperationException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorCode = "CONFLICT";
                    message = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    errorCode = "UNAUTHORIZED";
                    message = ex.Message;
                    break;
            }

            // Log: los 500 como error; los controlados como advertencia
            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Error no controlado. TraceId: {TraceId}", traceId);
            else
                _logger.LogWarning("Error controlado {ErrorCode}. TraceId: {TraceId}", errorCode, traceId);

            var response = new ErrorResponse
            {
                TraceId = traceId,
                ErrorCode = errorCode,
                Message = message,
                Details = details
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}