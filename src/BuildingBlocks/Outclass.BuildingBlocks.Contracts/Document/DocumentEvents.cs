namespace Outclass.BuildingBlocks.Contracts.Document;

public record DocumentCreatedEvent : IntegrationEvent
{
    public override string EventType => "document.created";
    public Guid DocumentId { get; init; }
    public string EntitySlug { get; init; } = default!;
}

public record DocumentUpdatedEvent : IntegrationEvent
{
    public override string EventType => "document.updated";
    public Guid DocumentId { get; init; }
    public string EntitySlug { get; init; } = default!;
}

public record DocumentDeletedEvent : IntegrationEvent
{
    public override string EventType => "document.deleted";
    public Guid DocumentId { get; init; }
    public string EntitySlug { get; init; } = default!;
}
