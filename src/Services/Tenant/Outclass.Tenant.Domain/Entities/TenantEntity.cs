using Outclass.BuildingBlocks.Domain;

namespace Outclass.Tenant.Domain.Entities;

public class TenantEntity : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public TenantStatus Status { get; private set; } = TenantStatus.Active;
    public TenantPlan Plan { get; private set; } = TenantPlan.Free;
    public string? LogoUrl { get; private set; }
    public string? Domain { get; private set; }
    public int MaxUsers { get; private set; } = 5;
    public long StorageLimitBytes { get; private set; } = 1073741824; // 1GB

    private readonly List<TenantSetting> _settings = new();
    public IReadOnlyCollection<TenantSetting> Settings => _settings.AsReadOnly();

    private TenantEntity() { }

    public static TenantEntity Create(string name, string slug, TenantPlan plan = TenantPlan.Free)
    {
        var tenant = new TenantEntity
        {
            Name = name,
            Slug = slug.ToLowerInvariant(),
            Plan = plan,
            MaxUsers = plan switch
            {
                TenantPlan.Free => 5,
                TenantPlan.Starter => 25,
                TenantPlan.Professional => 100,
                TenantPlan.Enterprise => int.MaxValue,
                _ => 5
            }
        };
        tenant.SetTenant(tenant.Id); // Self-referencing tenant
        tenant.AddDomainEvent(new TenantCreatedDomainEvent(tenant.Id, tenant.Name, tenant.Slug));
        return tenant;
    }

    public void Suspend(string reason)
    {
        Status = TenantStatus.Suspended;
        AddDomainEvent(new TenantSuspendedDomainEvent(Id, reason));
    }

    public void Activate()
    {
        Status = TenantStatus.Active;
    }

    public void UpdatePlan(TenantPlan plan)
    {
        Plan = plan;
        MaxUsers = plan switch
        {
            TenantPlan.Free => 5,
            TenantPlan.Starter => 25,
            TenantPlan.Professional => 100,
            TenantPlan.Enterprise => int.MaxValue,
            _ => MaxUsers
        };
    }

    public void AddSetting(string key, string value)
    {
        var existing = _settings.FirstOrDefault(s => s.Key == key);
        if (existing != null) existing.UpdateValue(value);
        else _settings.Add(new TenantSetting(Id, key, value));
    }

    public void SetDomain(string domain) => Domain = domain;
    public void SetLogo(string logoUrl) => LogoUrl = logoUrl;
}

public class TenantSetting
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantEntityId { get; private set; }
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;

    private TenantSetting() { }

    public TenantSetting(Guid tenantId, string key, string value)
    {
        TenantEntityId = tenantId;
        Key = key;
        Value = value;
    }

    public void UpdateValue(string value) => Value = value;
}

public enum TenantStatus { Active, Suspended, Deactivated }
public enum TenantPlan { Free, Starter, Professional, Enterprise }

public record TenantCreatedDomainEvent : DomainEvent
{
    public override string EventType => "tenant.created";
    public Guid TenantEntityId { get; }
    public string Name { get; }
    public string Slug { get; }

    public TenantCreatedDomainEvent(Guid tenantId, string name, string slug)
    {
        TenantEntityId = tenantId;
        Name = name;
        Slug = slug;
    }
}

public record TenantSuspendedDomainEvent : DomainEvent
{
    public override string EventType => "tenant.suspended";
    public Guid TenantEntityId { get; }
    public string Reason { get; }

    public TenantSuspendedDomainEvent(Guid tenantId, string reason)
    {
        TenantEntityId = tenantId;
        Reason = reason;
    }
}
