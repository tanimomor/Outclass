using MediatR;
using FluentValidation;
using Outclass.Metadata.Domain.Entities;

namespace Outclass.Metadata.Application.Commands;

public record CreateEntityDefinitionCommand : IRequest<EntityDefinitionDto>
{
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public List<CreateFieldDto> Fields { get; init; } = new();
}

public record CreateFieldDto
{
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public FieldType FieldType { get; init; }
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public int DisplayOrder { get; init; }
    public string? ValidationRules { get; init; }
}

public record EntityDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<FieldDefinitionDto> Fields { get; init; } = new();
}

public record FieldDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string FieldType { get; init; } = default!;
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
    public int DisplayOrder { get; init; }
}

public class CreateEntityDefinitionCommandValidator : AbstractValidator<CreateEntityDefinitionCommand>
{
    public CreateEntityDefinitionCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9_-]+$").WithMessage("Slug must be lowercase alphanumeric with dashes/underscores");
        RuleForEach(x => x.Fields).ChildRules(field =>
        {
            field.RuleFor(f => f.Name).NotEmpty().MaximumLength(200);
            field.RuleFor(f => f.Slug).NotEmpty().MaximumLength(100);
        });
    }
}
