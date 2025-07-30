using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Exceptions;

namespace TaskService.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ForbiddenAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (JobNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (NotificationSendException ex)
        {
            logger.LogError(ex, "Notification sending failed at {Path}. Exception: {Message}", context.Request.Path, ex.Message);
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsJsonAsync(new { error = "Notification service unavailable" });
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed at {Path}. Exception: {Message}", context.Request.Path, ex.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Database error" });
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception at {Path}. Message: {Message}", context.Request.Path, ex.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
        }
    }
}