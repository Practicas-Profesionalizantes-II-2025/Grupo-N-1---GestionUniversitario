using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ActivityTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ActiveUserTracker _tracker;

    public ActivityTrackingMiddleware(RequestDelegate next, ActiveUserTracker tracker)
    {
        _next = next;
        _tracker = tracker;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ejemplo: usar cookie de sesión o user id
        string sessionId = context.Request.Cookies[".AspNetCore.Session"] ?? context.Connection.Id;

        // opcional: contar solo si está autenticado
        // if (context.User?.Identity?.IsAuthenticated == true)
        {
            _tracker.MarkActivity(sessionId);
        }

        await _next(context);
    }
}
