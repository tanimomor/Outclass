using MediatR;
using Outclass.BuildingBlocks.Domain;
using Outclass.Tenant.Domain.Entities;

namespace Outclass.Tenant.Application.Queries.Handlers;

public class TenantQueryHandlers :
    IRequestHandler<GetTenantByIdQuery, TenantDto?>,
    IRequestHandler<GetTenantBySlugQuery, TenantDto?>,
    IRequestHandler<GetTenantsQuery, TenantsListDto>
{
    private readonly IRepository<TenantEntity> _tenantRepository;

    public TenantQueryHandlers(IRepository<TenantEntity> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, ct);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto?> Handle(GetTenantBySlugQuery request, CancellationToken ct)
    {
        var tenants = await _tenantRepository.FindAsync(t => t.Slug == request.Slug.ToLowerInvariant(), ct);
        var tenant = tenants.FirstOrDefault();
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantsListDto> Handle(GetTenantsQuery request, CancellationToken ct)
    {
        var all = await _tenantRepository.GetAllAsync(ct);
        return new TenantsListDto
        {
            Items = all.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .Select(MapToDto).ToList(),
            TotalCount = all.Count
        };
    }

    private static TenantDto MapToDto(TenantEntity t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Slug = t.Slug,
        Status = t.Status.ToString(),
        Plan = t.Plan.ToString(),
        MaxUsers = t.MaxUsers,
        CreatedAt = t.CreatedAt
    };
}
