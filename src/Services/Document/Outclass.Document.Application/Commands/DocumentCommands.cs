using System.Text.Json;
using MediatR;
using FluentValidation;

namespace Outclass.Document.Application.Commands;

public record CreateDocumentCommand : IRequest<DocumentDto>
{
    public string EntitySlug { get; init; } = default!;
    public JsonElement Data { get; init; }
}

public record UpdateDocumentCommand : IRequest<DocumentDto>
{
    public Guid Id { get; init; }
    public JsonElement Data { get; init; }
}

public record DeleteDocumentCommand(Guid Id) : IRequest<bool>;

public record DocumentDto
{
    public Guid Id { get; init; }
    public string EntitySlug { get; init; } = default!;
    public JsonElement Data { get; init; }
    public string? Status { get; init; }
    public int Version { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.EntitySlug).NotEmpty().MaximumLength(100);
    }
}
