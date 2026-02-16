using Outclass.BuildingBlocks.Domain;

namespace Outclass.Identity.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    private Role() { }

    public static Role Create(Guid tenantId, string name, string? description = null, bool isSystem = false)
    {
        var role = new Role
        {
            Name = name,
            Description = description,
            IsSystem = isSystem
        };
        role.SetTenant(tenantId);
        return role;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
