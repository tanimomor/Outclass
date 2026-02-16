using System.Text.Json;
using MediatR;
using Outclass.BuildingBlocks.Domain;
using Outclass.Document.Application.Commands;
using Outclass.Document.Domain.Entities;

namespace Outclass.Document.Application.Queries;

public record GetDocumentsQuery : IRequest<DocumentsListDto>
{
    public string EntitySlug { get; init; } = default!;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetDocumentByIdQuery(Guid Id) : IRequest<DocumentDto?>;

public record DocumentsListDto
{
    public IReadOnlyList<DocumentDto> Items { get; init; } = new List<DocumentDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public class GetDocumentsHandler : IRequestHandler<GetDocumentsQuery, DocumentsListDto>
{
    private readonly IRepository<DynamicDocument> _repository;
    public GetDocumentsHandler(IRepository<DynamicDocument> repository) => _repository = repository;

    public async Task<DocumentsListDto> Handle(GetDocumentsQuery request, CancellationToken ct)
    {
        var docs = await _repository.FindAsync(d => d.EntitySlug == request.EntitySlug.ToLowerInvariant(), ct);
        return new DocumentsListDto
        {
            Items = docs.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .Select(d => new DocumentDto
                {
                    Id = d.Id, EntitySlug = d.EntitySlug, Data = d.Data.RootElement.Clone(),
                    Status = d.Status, Version = d.Version, CreatedAt = d.CreatedAt, UpdatedAt = d.UpdatedAt
                }).ToList(),
            TotalCount = docs.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

public class GetDocumentByIdHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto?>
{
    private readonly IRepository<DynamicDocument> _repository;
    public GetDocumentByIdHandler(IRepository<DynamicDocument> repository) => _repository = repository;

    public async Task<DocumentDto?> Handle(GetDocumentByIdQuery request, CancellationToken ct)
    {
        var doc = await _repository.GetByIdAsync(request.Id, ct);
        if (doc == null) return null;
        return new DocumentDto
        {
            Id = doc.Id, EntitySlug = doc.EntitySlug, Data = doc.Data.RootElement.Clone(),
            Status = doc.Status, Version = doc.Version, CreatedAt = doc.CreatedAt, UpdatedAt = doc.UpdatedAt
        };
    }
}
