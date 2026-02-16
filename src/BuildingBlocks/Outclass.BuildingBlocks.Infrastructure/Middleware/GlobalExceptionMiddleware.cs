using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;

namespace Outclass.BuildingBlocks.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, new ErrorResponse
            {
                Code = ve.Code,
                Message = ve.Message,
                Errors = ve.Errors
            }),
            NotFoundException nf => (HttpStatusCode.NotFound, new ErrorResponse
            {
                Code = nf.Code,
                Message = nf.Message
            }),
            ForbiddenException fe => (HttpStatusCode.Forbidden, new ErrorResponse
            {
                Code = fe.Code,
                Message = fe.Message
            }),
            ConflictException ce => (HttpStatusCode.Conflict, new ErrorResponse
            {
                Code = ce.Code,
                Message = ce.Message
            }),
            TenantNotResolvedException te => (HttpStatusCode.BadRequest, new ErrorResponse
            {
                Code = te.Code,
                Message = te.Message
            }),
            _ => (HttpStatusCode.InternalServerError, new ErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An unexpected error occurred."
            })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Domain exception: {Code} - {Message}", response.Code, response.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public IDictionary<string, string[]>? Errors { get; set; }
    public string? TraceId { get; set; }
}
