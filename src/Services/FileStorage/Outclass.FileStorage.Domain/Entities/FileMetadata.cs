using Outclass.BuildingBlocks.Domain;

namespace Outclass.FileStorage.Domain.Entities;

public class FileMetadata : BaseEntity
{
    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; } = default!;
    public string? EntitySlug { get; private set; }
    public Guid? DocumentId { get; private set; }
    public string StorageProvider { get; private set; } = "local";

    private FileMetadata() { }

    public static FileMetadata Create(Guid tenantId, string fileName, string contentType, long sizeBytes, string storageKey, string? entitySlug = null, Guid? documentId = null)
    {
        var file = new FileMetadata
        {
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
            EntitySlug = entitySlug,
            DocumentId = documentId
        };
        file.SetTenant(tenantId);
        return file;
    }

    public void AssociateWithDocument(Guid documentId, string entitySlug)
    {
        DocumentId = documentId;
        EntitySlug = entitySlug;
    }
}
