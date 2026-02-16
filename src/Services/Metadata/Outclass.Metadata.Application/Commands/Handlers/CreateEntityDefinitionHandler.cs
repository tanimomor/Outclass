using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Domain;
using Outclass.Metadata.Domain.Entities;

namespace Outclass.Metadata.Application.Commands.Handlers;

public class CreateEntityDefinitionHandler : IRequestHandler<CreateEntityDefinitionCommand, EntityDefinitionDto>
{
    private readonly IRepository<EntityDefinition> _repository;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<CreateEntityDefinitionHandler> _logger;

    public CreateEntityDefinitionHandler(
        IRepository<EntityDefinition> repository,
        ITenantContext tenantContext,
        ILogger<CreateEntityDefinitionHandler> logger)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<EntityDefinitionDto> Handle(CreateEntityDefinitionCommand request, CancellationToken ct)
    {
        var existing = await _repository.FindAsync(e => e.Slug == request.Slug.ToLowerInvariant(), ct);
        if (existing.Count > 0)
            throw new ConflictException($"Entity definition with slug '{request.Slug}' already exists.");

        var entity = EntityDefinition.Create(
            _tenantContext.TenantId,
            request.Name,
            request.Slug,
            request.Description,
            request.Icon);

        foreach (var fieldDto in request.Fields.OrderBy(f => f.DisplayOrder))
        {
            entity.AddField(fieldDto.Name, fieldDto.Slug, fieldDto.FieldType,
                fieldDto.IsRequired, fieldDto.DefaultValue, fieldDto.DisplayOrder, fieldDto.ValidationRules);
        }

        await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Entity definition {Slug} created with {FieldCount} fields",
            entity.Slug, entity.Fields.Count);

        return MapToDto(entity);
    }

    private static EntityDefinitionDto MapToDto(EntityDefinition e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Slug = e.Slug,
        Description = e.Description,
        Icon = e.Icon,
        CreatedAt = e.CreatedAt,
        Fields = e.Fields.Select(f => new FieldDefinitionDto
        {
            Id = f.Id,
            Name = f.Name,
            Slug = f.Slug,
            FieldType = f.FieldType.ToString(),
            IsRequired = f.IsRequired,
            DefaultValue = f.DefaultValue,
            DisplayOrder = f.DisplayOrder
        }).ToList()
    };
}
