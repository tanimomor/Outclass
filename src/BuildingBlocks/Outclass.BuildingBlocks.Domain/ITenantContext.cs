namespace Outclass.BuildingBlocks.Domain;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool IsResolved { get; }
}

public interface ICurrentUser
{
    string UserId { get; }
    string Email { get; }
    Guid TenantId { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
}
