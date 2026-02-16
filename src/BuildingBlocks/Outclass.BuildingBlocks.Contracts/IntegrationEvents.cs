namespace Outclass.BuildingBlocks.Contracts;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    Guid TenantId { get; }
}

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public abstract string EventType { get; }
    public Guid TenantId { get; init; }
}
