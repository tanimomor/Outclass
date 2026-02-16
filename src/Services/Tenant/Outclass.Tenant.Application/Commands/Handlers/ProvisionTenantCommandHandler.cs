using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Application.EventBus;
using Outclass.BuildingBlocks.Contracts.Tenant;
using Outclass.BuildingBlocks.Domain;
using Outclass.Tenant.Domain.Entities;

namespace Outclass.Tenant.Application.Commands.Handlers;

public class ProvisionTenantCommandHandler : IRequestHandler<ProvisionTenantCommand, ProvisionTenantResult>
{
    private readonly IRepository<TenantEntity> _tenantRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ProvisionTenantCommandHandler> _logger;

    public ProvisionTenantCommandHandler(
        IRepository<TenantEntity> tenantRepository,
        IEventBus eventBus,
        ILogger<ProvisionTenantCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<ProvisionTenantResult> Handle(ProvisionTenantCommand request, CancellationToken ct)
    {
        // Check slug uniqueness
        var existing = await _tenantRepository.FindAsync(t => t.Slug == request.Slug.ToLowerInvariant(), ct);
        if (existing.Count > 0)
            throw new ConflictException($"Tenant with slug '{request.Slug}' already exists.");

        var tenant = TenantEntity.Create(request.Name, request.Slug, request.Plan);
        await _tenantRepository.AddAsync(tenant, ct);

        // Publish integration event
        await _eventBus.PublishAsync(new TenantProvisionedEvent
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            Slug = tenant.Slug
        }, ct);

        _logger.LogInformation("Tenant {TenantId} provisioned with slug {Slug}", tenant.Id, tenant.Slug);

        return new ProvisionTenantResult
        {
            TenantId = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug
        };
    }
}
