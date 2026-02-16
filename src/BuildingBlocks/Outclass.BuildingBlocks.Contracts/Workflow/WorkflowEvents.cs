namespace Outclass.BuildingBlocks.Contracts.Workflow;

public record WorkflowTransitionedEvent : IntegrationEvent
{
    public override string EventType => "workflow.transitioned";
    public Guid DocumentId { get; init; }
    public string FromState { get; init; } = default!;
    public string ToState { get; init; } = default!;
    public string? TransitionedBy { get; init; }
}
