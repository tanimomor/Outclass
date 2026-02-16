namespace Outclass.BuildingBlocks.Contracts.Tenant;

public record TenantProvisionedEvent : IntegrationEvent
{
    public override string EventType => "tenant.provisioned";
    public string TenantName { get; init; } = default!;
    public string Slug { get; init; } = default!;
}

public record TenantSuspendedEvent : IntegrationEvent
{
    public override string EventType => "tenant.suspended";
    public string Reason { get; init; } = default!;
}
