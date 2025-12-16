using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ProductCatalog.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var result = GetErrorResult(exception);
            context.Response.StatusCode = result.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };

            var jsonResponse = JsonSerializer.Serialize(result, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ErrorResponse GetErrorResult(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => new ErrorResponse
                {
                    StatusCode = validationEx.StatusCode,
                    Message = validationEx.Message,
                    ErrorCode = validationEx.ErrorCode,
                    ValidationErrors = validationEx.Errors,
                    Details = _env.IsDevelopment() ? exception.StackTrace : null
                },

                NotFoundException notFoundEx when notFoundEx.ErrorCode == ErrorCodes.ProductNotFound =>
                    new ErrorResponse
                    {
                        StatusCode = notFoundEx.StatusCode,
                        Message = notFoundEx.Message,
                        ErrorCode = notFoundEx.ErrorCode,
                        Details = _env.IsDevelopment() ? exception.StackTrace : null
                    },

                BusinessRuleException businessEx => new ErrorResponse
                {
                    StatusCode = businessEx.StatusCode,
                    Message = businessEx.Message,
                    ErrorCode = businessEx.ErrorCode,
                    Details = _env.IsDevelopment() ? exception.StackTrace : null
                },

                BaseException baseEx => new ErrorResponse
                {
                    StatusCode = baseEx.StatusCode,
                    Message = baseEx.Message,
                    ErrorCode = baseEx.ErrorCode,
                    Details = _env.IsDevelopment() ? exception.StackTrace : null
                },

                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _env.IsDevelopment() ? exception.Message : "An internal server error occurred",
                    ErrorCode = ErrorCodes.InternalServerError,
                    Details = _env.IsDevelopment() ? exception.StackTrace : null
                }
            };
        }
        //private ErrorResponse GetErrorResult(Exception exception)
        //{
        //    return exception switch
        //    {
        //        ValidationException validationEx => new ErrorResponse
        //        {
        //            StatusCode = validationEx.StatusCode,
        //            Message = validationEx.Message,
        //            ErrorCode = validationEx.ErrorCode,
        //            ValidationErrors = validationEx.Errors,
        //            Details = _env.IsDevelopment() ? exception.StackTrace : null
        //        },

        //        BaseException baseEx => new ErrorResponse
        //        {
        //            StatusCode = baseEx.StatusCode,
        //            Message = baseEx.Message,
        //            ErrorCode = baseEx.ErrorCode,
        //            Details = _env.IsDevelopment() ? exception.StackTrace : null
        //        },



        //        _ => new ErrorResponse
        //        {
        //            StatusCode = (int)HttpStatusCode.InternalServerError,
        //            Message = _env.IsDevelopment() ? exception.Message : "An internal server error occurred",
        //            ErrorCode = ErrorCodes.InternalServerError,
        //            Details = _env.IsDevelopment() ? exception.StackTrace : null
        //        }
        //    };
        //}
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}