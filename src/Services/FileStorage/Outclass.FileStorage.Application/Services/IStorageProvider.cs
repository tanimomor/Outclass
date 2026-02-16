namespace Outclass.FileStorage.Application.Services;

public interface IStorageProvider
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string storageKey, CancellationToken ct = default);
    Task DeleteAsync(string storageKey, CancellationToken ct = default);
    Task<string> GetPresignedUrlAsync(string storageKey, TimeSpan expiry, CancellationToken ct = default);
}
