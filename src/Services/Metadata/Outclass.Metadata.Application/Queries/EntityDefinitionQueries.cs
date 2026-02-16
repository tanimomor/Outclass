using MediatR;
using Outclass.BuildingBlocks.Domain;
using Outclass.Metadata.Application.Commands;
using Outclass.Metadata.Domain.Entities;

namespace Outclass.Metadata.Application.Queries;

public record GetEntityDefinitionsQuery : IRequest<List<EntityDefinitionDto>>;
public record GetEntityDefinitionBySlugQuery(string Slug) : IRequest<EntityDefinitionDto?>;

public class GetEntityDefinitionsHandler : IRequestHandler<GetEntityDefinitionsQuery, List<EntityDefinitionDto>>
{
    private readonly IRepository<EntityDefinition> _repository;

    public GetEntityDefinitionsHandler(IRepository<EntityDefinition> repository) => _repository = repository;

    public async Task<List<EntityDefinitionDto>> Handle(GetEntityDefinitionsQuery request, CancellationToken ct)
    {
        var entities = await _repository.GetAllAsync(ct);
        return entities.Select(e => new EntityDefinitionDto
        {
            Id = e.Id, Name = e.Name, Slug = e.Slug, Description = e.Description,
            Icon = e.Icon, CreatedAt = e.CreatedAt,
            Fields = e.Fields.Select(f => new FieldDefinitionDto
            {
                Id = f.Id, Name = f.Name, Slug = f.Slug, FieldType = f.FieldType.ToString(),
                IsRequired = f.IsRequired, DefaultValue = f.DefaultValue, DisplayOrder = f.DisplayOrder
            }).OrderBy(f => f.DisplayOrder).ToList()
        }).ToList();
    }
}

public class GetEntityDefinitionBySlugHandler : IRequestHandler<GetEntityDefinitionBySlugQuery, EntityDefinitionDto?>
{
    private readonly IRepository<EntityDefinition> _repository;

    public GetEntityDefinitionBySlugHandler(IRepository<EntityDefinition> repository) => _repository = repository;

    public async Task<EntityDefinitionDto?> Handle(GetEntityDefinitionBySlugQuery request, CancellationToken ct)
    {
        var entities = await _repository.FindAsync(e => e.Slug == request.Slug.ToLowerInvariant(), ct);
        var entity = entities.FirstOrDefault();
        if (entity == null) return null;
        return new EntityDefinitionDto
        {
            Id = entity.Id, Name = entity.Name, Slug = entity.Slug, Description = entity.Description,
            Icon = entity.Icon, CreatedAt = entity.CreatedAt,
            Fields = entity.Fields.Select(f => new FieldDefinitionDto
            {
                Id = f.Id, Name = f.Name, Slug = f.Slug, FieldType = f.FieldType.ToString(),
                IsRequired = f.IsRequired, DefaultValue = f.DefaultValue, DisplayOrder = f.DisplayOrder
            }).OrderBy(f => f.DisplayOrder).ToList()
        };
    }
}
