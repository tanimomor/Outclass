namespace Outclass.BuildingBlocks.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid TenantId { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void SetAudit(string userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCreatedBy(string userId)
    {
        CreatedBy = userId;
        UpdatedBy = userId;
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
