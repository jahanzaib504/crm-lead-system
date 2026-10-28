using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        AppDbContext db)
    {
        var request = context.Request;
        var path = request.Path.Value ?? string.Empty;

        // Skip logging for OPTIONS preflight, Swagger, static files, and the audit-logs endpoint itself
        if (request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/audit-logs", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/favicon.ico", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            try
            {
                //var user = context.User;
                //var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                //    ?? user?.FindFirst("sub")?.Value
                //    ?? user?.FindFirst("nameid")?.Value;

                //int? parsedUserId = int.TryParse(userIdClaim, out int uid) ? uid : null;

                //// Extract entity name and entity id from path segments (e.g. /leads/42 -> leads, 42)
                //var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                //string entityName = segments.Length > 0 ? segments[0] : "http_request";
                //int entityId = 0;
                //if (segments.Length > 1 && int.TryParse(segments[1], out int idVal))
                //{
                //    entityId = idVal;
                //}

                //// Action max length is 20
                //string action = request.Method.ToUpperInvariant();
                //if (action.Length > 20) action = action.Substring(0, 20);

                //var log = new AuditLog
                //{
                //    PerformedByUserId = parsedUserId,
                //    Action = action,
                //    EntityName = entityName.Length > 100 ? entityName.Substring(0, 100) : entityName,
                //    EntityId = entityId,
                //    ChangesJson = JsonSerializer.Serialize(new
                //    {
                //        method = request.Method,
                //        path = path,
                //        queryString = request.QueryString.HasValue ? request.QueryString.Value : null,
                //        statusCode = context.Response.StatusCode,
                //        durationMs = stopwatch.ElapsedMilliseconds
                //    }),
                //    Timestamp = DateTime.UtcNow
                //};

                //db.AuditLogs.Add(log);
                //await db.SaveChangesAsync();

                //_logger.LogInformation(
                //    "[AUDIT LOG] {Method} {Path} | Status: {Status} | UserId: {UserId} | Duration: {Duration}ms",
                //    request.Method,
                //    path,
                //    context.Response.StatusCode,
                //    userIdClaim ?? "Anonymous",
                //    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist audit log for {Method} {Path}", request.Method, path);
            }
        }
    }
}
