using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outclass.BuildingBlocks.Domain;
using Outclass.FileStorage.Application.Services;
using Outclass.FileStorage.Domain.Entities;

namespace Outclass.FileStorage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IRepository<FileMetadata> _repository;
    private readonly IStorageProvider _storageProvider;
    private readonly ITenantContext _tenantContext;

    public FilesController(IRepository<FileMetadata> repository, IStorageProvider storageProvider, ITenantContext tenantContext)
    {
        _repository = repository;
        _storageProvider = storageProvider;
        _tenantContext = tenantContext;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(52428800)] // 50MB
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string? entitySlug, [FromQuery] Guid? documentId, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var storageKey = await _storageProvider.UploadAsync(stream, file.FileName, file.ContentType, ct);

        var metadata = FileMetadata.Create(
            _tenantContext.TenantId,
            file.FileName,
            file.ContentType,
            file.Length,
            storageKey,
            entitySlug,
            documentId);

        await _repository.AddAsync(metadata, ct);

        return Ok(new { metadata.Id, metadata.FileName, metadata.SizeBytes, metadata.StorageKey });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMetadata(Guid id, CancellationToken ct)
    {
        var metadata = await _repository.GetByIdAsync(id, ct);
        if (metadata == null) return NotFound();
        return Ok(metadata);
    }

    [HttpGet("download/{id:guid}")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var metadata = await _repository.GetByIdAsync(id, ct);
        if (metadata == null) return NotFound();

        var stream = await _storageProvider.DownloadAsync(metadata.StorageKey, ct);
        return File(stream, metadata.ContentType, metadata.FileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var metadata = await _repository.GetByIdAsync(id, ct);
        if (metadata == null) return NotFound();

        await _storageProvider.DeleteAsync(metadata.StorageKey, ct);
        await _repository.DeleteAsync(metadata, ct);
        return NoContent();
    }
}
