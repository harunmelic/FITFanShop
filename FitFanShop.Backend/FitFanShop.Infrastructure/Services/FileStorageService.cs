using FitFanShop.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FitFanShop.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string mimeType, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                throw new InvalidOperationException("WebRootPath is not configured. Ensure wwwroot folder exists.");
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images");
            
            _logger.LogInformation("Creating directory: {UploadsFolder}", uploadsFolder);
            Directory.CreateDirectory(uploadsFolder);
            
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            _logger.LogInformation("Saving file to: {FilePath}", filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream, ct);
            }

            var fileUrl = $"/uploads/images/{uniqueFileName}";
            _logger.LogInformation("File saved successfully: {FileUrl}", fileUrl);
            
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", fileName);
            throw;
        }
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                _logger.LogWarning("WebRootPath is not configured. Cannot delete file: {FileUrl}", fileUrl);
                return Task.CompletedTask;
            }

            var filePath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            throw;
        }
    }

    public bool FileExists(string fileUrl)
    {
        if (string.IsNullOrEmpty(_env.WebRootPath))
            return false;

        var filePath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/'));
        return File.Exists(filePath);
    }
}
