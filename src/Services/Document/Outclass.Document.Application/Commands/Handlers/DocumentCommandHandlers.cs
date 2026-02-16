using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Outclass.BuildingBlocks.Application.EventBus;
using Outclass.BuildingBlocks.Contracts.Document;
using Outclass.BuildingBlocks.Domain;
using Outclass.Document.Domain.Entities;

namespace Outclass.Document.Application.Commands.Handlers;

public class DocumentCommandHandlers :
    IRequestHandler<CreateDocumentCommand, DocumentDto>,
    IRequestHandler<UpdateDocumentCommand, DocumentDto>,
    IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly IRepository<DynamicDocument> _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<DocumentCommandHandlers> _logger;

    public DocumentCommandHandlers(
        IRepository<DynamicDocument> repository,
        ITenantContext tenantContext,
        IEventBus eventBus,
        ILogger<DocumentCommandHandlers> logger)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<DocumentDto> Handle(CreateDocumentCommand request, CancellationToken ct)
    {
        var jsonDoc = JsonDocument.Parse(request.Data.GetRawText());
        var document = DynamicDocument.Create(_tenantContext.TenantId, request.EntitySlug, jsonDoc);
        await _repository.AddAsync(document, ct);

        await _eventBus.PublishAsync(new DocumentCreatedEvent
        {
            TenantId = _tenantContext.TenantId,
            DocumentId = document.Id,
            EntitySlug = document.EntitySlug
        }, ct);

        _logger.LogInformation("Document {DocumentId} created for entity {EntitySlug}", document.Id, document.EntitySlug);
        return MapToDto(document);
    }

    public async Task<DocumentDto> Handle(UpdateDocumentCommand request, CancellationToken ct)
    {
        var document = await _repository.GetByIdAsync(request.Id, ct)
                       ?? throw new NotFoundException("Document", request.Id);

        var jsonDoc = JsonDocument.Parse(request.Data.GetRawText());
        document.UpdateData(jsonDoc);
        await _repository.UpdateAsync(document, ct);

        await _eventBus.PublishAsync(new DocumentUpdatedEvent
        {
            TenantId = _tenantContext.TenantId,
            DocumentId = document.Id,
            EntitySlug = document.EntitySlug
        }, ct);

        return MapToDto(document);
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken ct)
    {
        var document = await _repository.GetByIdAsync(request.Id, ct)
                       ?? throw new NotFoundException("Document", request.Id);

        document.SoftDelete();
        await _repository.UpdateAsync(document, ct);

        await _eventBus.PublishAsync(new DocumentDeletedEvent
        {
            TenantId = _tenantContext.TenantId,
            DocumentId = document.Id,
            EntitySlug = document.EntitySlug
        }, ct);

        return true;
    }

    private static DocumentDto MapToDto(DynamicDocument d) => new()
    {
        Id = d.Id,
        EntitySlug = d.EntitySlug,
        Data = d.Data.RootElement.Clone(),
        Status = d.Status,
        Version = d.Version,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };
}
