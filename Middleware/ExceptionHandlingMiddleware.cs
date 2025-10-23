using System.Net;
using System.Text.Json;

namespace Recipy.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<Exception> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<Exception> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by middleware");
            await HandleExceptionsAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionsAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            error = "An unexpected error occurred",
            details = ex.Message
        };

        await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(response));
    
    }
}