using Outclass.FileStorage.Application.Services;

namespace Outclass.FileStorage.Infrastructure.Storage;

public class LocalStorageProvider : IStorageProvider
{
    private readonly string _basePath;

    public LocalStorageProvider(string basePath = "/tmp/outclass-files")
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var key = $"{Guid.NewGuid()}/{fileName}";
        var fullPath = Path.Combine(_basePath, key);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, ct);

        return key;
    }

    public async Task<Stream> DownloadAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {storageKey}");

        return await Task.FromResult(File.OpenRead(fullPath) as Stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<string> GetPresignedUrlAsync(string storageKey, TimeSpan expiry, CancellationToken ct = default)
    {
        // Local storage doesn't support presigned URLs; return local path
        return Task.FromResult($"/api/files/download/{storageKey}");
    }
}
