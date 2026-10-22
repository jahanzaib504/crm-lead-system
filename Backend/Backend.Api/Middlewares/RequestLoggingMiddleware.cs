using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Backend.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        AppDbContext db)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var userId = context.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var log = new AuditLog
            {
                PerformedByUserId =
                    int.TryParse(userId, out int parsedId)
                        ? parsedId
                        : null,

                Action = "http.request",
                EntityName = "http_request",

                ChangesJson = JsonSerializer.Serialize(new
                {
                    method = request.Method,
                    path = request.Path.Value,
                    statusCode = context.Response.StatusCode,
                    durationMs = stopwatch.ElapsedMilliseconds
                }),

                Timestamp = DateTime.UtcNow
            };

            db.AuditLogs.Add(log);

            await db.SaveChangesAsync();

            Console.WriteLine(
                $"[AUDIT LOG] {request.Method} {request.Path} | " +
                $"Status: {context.Response.StatusCode} | " +
                $"UserId: {userId ?? "Anonymous"} | " +
                $"Duration: {stopwatch.ElapsedMilliseconds}ms | " +
                $"Timestamp: {log.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
            );
        }
    }
}