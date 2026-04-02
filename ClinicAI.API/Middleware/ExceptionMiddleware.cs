using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClinicAI.API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                var traceid = context.TraceIdentifier;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex, "❌ Unhandled exception | TraceId: {TraceId}", traceid+ex.Message);

                // Handle FluentValidation exceptions with structured response
                if (ex is FluentValidation.ValidationException vex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    // Group errors by property name
                    var errorList = vex.Errors
                                         .Select(e => e.ErrorMessage)
                                         .Distinct()
                                         .ToList();

                    var response = new
                    {
                        isSuccess = false,
                        message = errorList.FirstOrDefault(),
                        errors = errorList
                    };

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = ex.Message
                    }));
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
