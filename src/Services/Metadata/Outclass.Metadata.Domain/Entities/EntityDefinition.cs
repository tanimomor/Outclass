using Outclass.BuildingBlocks.Domain;

namespace Outclass.Metadata.Domain.Entities;

public class EntityDefinition : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public bool IsSystem { get; private set; }

    private readonly List<FieldDefinition> _fields = new();
    public IReadOnlyCollection<FieldDefinition> Fields => _fields.AsReadOnly();

    private EntityDefinition() { }

    public static EntityDefinition Create(Guid tenantId, string name, string slug, string? description = null, string? icon = null)
    {
        var entity = new EntityDefinition
        {
            Name = name,
            Slug = slug.ToLowerInvariant(),
            Description = description,
            Icon = icon
        };
        entity.SetTenant(tenantId);
        return entity;
    }

    public FieldDefinition AddField(string name, string slug, FieldType fieldType, bool isRequired = false, string? defaultValue = null, int displayOrder = 0, string? validationRules = null)
    {
        if (_fields.Any(f => f.Slug == slug.ToLowerInvariant()))
            throw new ConflictException($"Field with slug '{slug}' already exists on entity '{Name}'.");

        var field = FieldDefinition.Create(Id, TenantId, name, slug, fieldType, isRequired, defaultValue, displayOrder, validationRules);
        _fields.Add(field);
        return field;
    }

    public void RemoveField(Guid fieldId)
    {
        var field = _fields.FirstOrDefault(f => f.Id == fieldId)
                    ?? throw new NotFoundException("FieldDefinition", fieldId);
        field.MarkAsDeleted();
    }

    public void Update(string name, string? description, string? icon)
    {
        Name = name;
        Description = description;
        Icon = icon;
    }
}

public class FieldDefinition : BaseEntity
{
    public Guid EntityDefinitionId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public FieldType FieldType { get; private set; }
    public bool IsRequired { get; private set; }
    public string? DefaultValue { get; private set; }
    public int DisplayOrder { get; private set; }
    public string? ValidationRules { get; private set; } // JSON
    public string? Options { get; private set; } // JSON for select/radio

    private FieldDefinition() { }

    public static FieldDefinition Create(Guid entityDefinitionId, Guid tenantId, string name, string slug,
        FieldType fieldType, bool isRequired, string? defaultValue, int displayOrder, string? validationRules)
    {
        var field = new FieldDefinition
        {
            EntityDefinitionId = entityDefinitionId,
            Name = name,
            Slug = slug.ToLowerInvariant(),
            FieldType = fieldType,
            IsRequired = isRequired,
            DefaultValue = defaultValue,
            DisplayOrder = displayOrder,
            ValidationRules = validationRules
        };
        field.SetTenant(tenantId);
        return field;
    }

    public void Update(string name, bool isRequired, string? defaultValue, int displayOrder, string? validationRules, string? options)
    {
        Name = name;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        DisplayOrder = displayOrder;
        ValidationRules = validationRules;
        Options = options;
    }
}

public enum FieldType
{
    Text,
    TextArea,
    Number,
    Decimal,
    Boolean,
    Date,
    DateTime,
    Email,
    Url,
    Phone,
    Select,
    MultiSelect,
    Radio,
    Checkbox,
    File,
    Image,
    RichText,
    Json,
    Reference
}
