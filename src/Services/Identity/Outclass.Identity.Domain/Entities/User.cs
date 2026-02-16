using Outclass.BuildingBlocks.Domain;

namespace Outclass.Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { }

    public static User Create(Guid tenantId, string email, string passwordHash, string firstName, string lastName)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName
        };
        user.SetTenant(tenantId);
        user.AddDomainEvent(new UserCreatedDomainEvent(user.Id, user.Email, tenantId));
        return user;
    }

    public void AssignRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id)) return;
        _userRoles.Add(new UserRole { UserId = Id, RoleId = role.Id, Role = role });
        AddDomainEvent(new UserRoleAssignedDomainEvent(Id, role.Name, TenantId));
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
}

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}

public record UserCreatedDomainEvent : DomainEvent
{
    public override string EventType => "identity.user.created";
    public Guid UserId { get; }
    public string Email { get; }
    public Guid TenantId { get; }

    public UserCreatedDomainEvent(Guid userId, string email, Guid tenantId)
    {
        UserId = userId;
        Email = email;
        TenantId = tenantId;
    }
}

public record UserRoleAssignedDomainEvent : DomainEvent
{
    public override string EventType => "identity.user.role_assigned";
    public Guid UserId { get; }
    public string RoleName { get; }
    public Guid TenantId { get; }

    public UserRoleAssignedDomainEvent(Guid userId, string roleName, Guid tenantId)
    {
        UserId = userId;
        RoleName = roleName;
        TenantId = tenantId;
    }
}
