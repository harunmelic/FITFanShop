namespace FitFanShop.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string mimeType, CancellationToken ct = default);
    Task DeleteFileAsync(string fileUrl, CancellationToken ct = default);
    bool FileExists(string fileUrl);
}
