using FitFanShop.Application.Abstractions;
using FitFanShop.Domain.Entities.Notifications;

namespace FitFanShop.API.Middleware;

public class ActivityLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLogMiddleware> _logger;

    public ActivityLogMiddleware(RequestDelegate next, ILogger<ActivityLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAppDbContext dbContext, IAppCurrentUser currentUser)
    {
        await _next(context);

        if (ShouldLog(context, currentUser))
        {
            try
            {
                var actionDescription = GetActionDescription(context);
                
                var activityLog = new ActivityLogEntity
                {
                    UserId = currentUser.UserId!.Value,
                    ActionDescription = actionDescription,
                    CreatedAtUtc = DateTime.UtcNow
                };

                dbContext.ActivityLogs.Add(activityLog);
                await dbContext.SaveChangesAsync(default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity for user {UserId}", currentUser.UserId);
            }
        }
    }

    private bool ShouldLog(HttpContext context, IAppCurrentUser currentUser)
    {
        if (!currentUser.UserId.HasValue)
            return false;

        if (context.Response.StatusCode >= 400)
            return false;

        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        
        if (path.Contains("/activity-logs"))
            return false;

        if (context.Request.Method == "GET")
            return false;

        return true;
    }

    private string GetActionDescription(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";

        return method switch
        {
            "POST" when path.Contains("/auth/login") => "User logged in",
            "POST" when path.Contains("/auth/register") => "User registered",
            "POST" when path.Contains("/orders") => "Created order",
            "POST" when path.Contains("/events") => "Created event",
            "POST" when path.Contains("/discounts") => "Created discount",
            "POST" when path.Contains("/cart") => "Added item to cart",
            "POST" when path.Contains("/wishlist") => "Added item to wishlist",
            "PUT" when path.Contains("/users/me") => "Updated profile",
            "PUT" when path.Contains("/events") => "Updated event",
            "PUT" when path.Contains("/discounts") => "Updated discount",
            "DELETE" when path.Contains("/users/me") => "Deleted account",
            "DELETE" when path.Contains("/events") => "Deleted event",
            "DELETE" when path.Contains("/discounts") => "Deleted discount",
            "DELETE" when path.Contains("/cart") => "Removed item from cart",
            "PATCH" when path.Contains("/orders") && path.Contains("/cancel") => "Cancelled order",
            _ => $"{method} {path}"
        };
    }
}
