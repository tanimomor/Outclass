namespace Outclass.BuildingBlocks.Contracts.Identity;

public record UserRegisteredEvent : IntegrationEvent
{
    public override string EventType => "identity.user.registered";
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
}

public record UserRoleAssignedEvent : IntegrationEvent
{
    public override string EventType => "identity.user.role_assigned";
    public Guid UserId { get; init; }
    public string RoleName { get; init; } = default!;
}
