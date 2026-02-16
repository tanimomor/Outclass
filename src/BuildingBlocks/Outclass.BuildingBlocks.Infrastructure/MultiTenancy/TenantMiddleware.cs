using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;

namespace Outclass.BuildingBlocks.Infrastructure.MultiTenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;
    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/healthz",
        "/ready",
        "/.well-known"
    };

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var tenantContext = context.RequestServices.GetService(typeof(ITenantContext)) as ITenantContext;

        if (tenantContext == null || !tenantContext.IsResolved)
        {
            _logger.LogWarning("Tenant context not resolved for path {Path}", path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant context required" });
            return;
        }

        _logger.LogDebug("Tenant {TenantId} resolved for request {Path}", tenantContext.TenantId, path);
        await _next(context);
    }
}
