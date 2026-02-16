using MediatR;

namespace Outclass.Tenant.Application.Queries;

public record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto?>;
public record GetTenantBySlugQuery(string Slug) : IRequest<TenantDto?>;
public record GetTenantsQuery(int Page = 1, int PageSize = 20) : IRequest<TenantsListDto>;

public record TenantDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string Status { get; init; } = default!;
    public string Plan { get; init; } = default!;
    public int MaxUsers { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record TenantsListDto
{
    public IReadOnlyList<TenantDto> Items { get; init; } = new List<TenantDto>();
    public int TotalCount { get; init; }
}
