using Backend.Application.Dtos;
using Backend.Application.Dtos.Api;
using Backend.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Api.Middlewares;
public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            CustomException customEx => (customEx.StatusCode, customEx.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
        }

        var problemDetails = new ApiError
        {
            Status = statusCode,
            Message = title,
            TraceId = httpContext.TraceIdentifier
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Mark as handled
    }
}

