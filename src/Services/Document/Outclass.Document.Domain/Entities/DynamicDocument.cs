using System.Text.Json;
using Outclass.BuildingBlocks.Domain;

namespace Outclass.Document.Domain.Entities;

public class DynamicDocument : BaseEntity
{
    public string EntitySlug { get; private set; } = default!;
    public JsonDocument Data { get; private set; } = default!;
    public string? Status { get; private set; }
    public int Version { get; private set; } = 1;

    private DynamicDocument() { }

    public static DynamicDocument Create(Guid tenantId, string entitySlug, JsonDocument data, string? status = null)
    {
        var doc = new DynamicDocument
        {
            EntitySlug = entitySlug.ToLowerInvariant(),
            Data = data,
            Status = status ?? "draft"
        };
        doc.SetTenant(tenantId);
        doc.AddDomainEvent(new DocumentCreatedDomainEvent(doc.Id, doc.EntitySlug, tenantId));
        return doc;
    }

    public void UpdateData(JsonDocument newData)
    {
        Data = newData;
        Version++;
        AddDomainEvent(new DocumentUpdatedDomainEvent(Id, EntitySlug, TenantId));
    }

    public void SetStatus(string status)
    {
        var oldStatus = Status;
        Status = status;
        AddDomainEvent(new DocumentStatusChangedDomainEvent(Id, EntitySlug, oldStatus, status, TenantId));
    }

    public void SoftDelete()
    {
        MarkAsDeleted();
        AddDomainEvent(new DocumentDeletedDomainEvent(Id, EntitySlug, TenantId));
    }
}

public record DocumentCreatedDomainEvent : DomainEvent
{
    public override string EventType => "document.created";
    public Guid DocumentId { get; }
    public string EntitySlug { get; }
    public Guid TenantIdValue { get; }

    public DocumentCreatedDomainEvent(Guid documentId, string entitySlug, Guid tenantId)
    {
        DocumentId = documentId;
        EntitySlug = entitySlug;
        TenantIdValue = tenantId;
    }
}

public record DocumentUpdatedDomainEvent : DomainEvent
{
    public override string EventType => "document.updated";
    public Guid DocumentId { get; }
    public string EntitySlug { get; }
    public Guid TenantIdValue { get; }

    public DocumentUpdatedDomainEvent(Guid documentId, string entitySlug, Guid tenantId)
    {
        DocumentId = documentId;
        EntitySlug = entitySlug;
        TenantIdValue = tenantId;
    }
}

public record DocumentStatusChangedDomainEvent : DomainEvent
{
    public override string EventType => "document.status_changed";
    public Guid DocumentId { get; }
    public string EntitySlug { get; }
    public string? OldStatus { get; }
    public string NewStatus { get; }
    public Guid TenantIdValue { get; }

    public DocumentStatusChangedDomainEvent(Guid documentId, string entitySlug, string? oldStatus, string newStatus, Guid tenantId)
    {
        DocumentId = documentId;
        EntitySlug = entitySlug;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        TenantIdValue = tenantId;
    }
}

public record DocumentDeletedDomainEvent : DomainEvent
{
    public override string EventType => "document.deleted";
    public Guid DocumentId { get; }
    public string EntitySlug { get; }
    public Guid TenantIdValue { get; }

    public DocumentDeletedDomainEvent(Guid documentId, string entitySlug, Guid tenantId)
    {
        DocumentId = documentId;
        EntitySlug = entitySlug;
        TenantIdValue = tenantId;
    }
}
