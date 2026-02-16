using Microsoft.AspNetCore.Http;
using Outclass.BuildingBlocks.Domain;
using System.Security.Claims;

namespace Outclass.BuildingBlocks.Infrastructure.MultiTenancy;

public class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return Guid.Empty;

            // Try header first (set by gateway)
            var tenantHeader = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantHeader) && Guid.TryParse(tenantHeader, out var headerTenantId))
                return headerTenantId;

            // Fallback to JWT claim
            var tenantClaim = httpContext.User.FindFirstValue("tenant_id");
            if (!string.IsNullOrEmpty(tenantClaim) && Guid.TryParse(tenantClaim, out var claimTenantId))
                return claimTenantId;

            return Guid.Empty;
        }
    }

    public bool IsResolved => TenantId != Guid.Empty;
}

public class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    public Guid TenantId
    {
        get
        {
            var claim = User?.FindFirstValue("tenant_id");
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
    public IReadOnlyList<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
