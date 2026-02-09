using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Media;
using MediatR;

namespace FitFanShop.Application.Modules.Media.Images.Commands.UploadImage;

public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, UploadImageResponseDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IFileStorageService _fileStorage;

    private static readonly string[] AllowedMimeTypes = 
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    public UploadImageCommandHandler(IAppDbContext ctx, IFileStorageService fileStorage)
    {
        _ctx = ctx;
        _fileStorage = fileStorage;
    }

    public async Task<UploadImageResponseDto> Handle(UploadImageCommand command, CancellationToken ct)
    {
        if (command.FileStream == null || command.FileStream.Length == 0)
            throw new FitFanShopBusinessRuleException("FileRequired", "No file uploaded");

        if (string.IsNullOrWhiteSpace(command.FileName))
            throw new FitFanShopBusinessRuleException("FileNameRequired", "File name is required");

        if (!AllowedMimeTypes.Contains(command.MimeType))
            throw new FitFanShopBusinessRuleException("InvalidFileType", "Invalid file type. Allowed types: JPEG, PNG, GIF, WEBP");

        if (command.FileSize > ImageEntity.Constraints.MaxFileSize)
            throw new FitFanShopBusinessRuleException("FileTooLarge", "File too large. Maximum size is 10MB");

        var fileUrl = await _fileStorage.SaveFileAsync(command.FileStream, command.FileName, command.MimeType, ct);

        var image = new ImageEntity
        {
            FileName = command.FileName,
            FileUrl = fileUrl,
            FileSize = command.FileSize,
            MimeType = command.MimeType,
            UploadDateUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Images.Add(image);
        await _ctx.SaveChangesAsync(ct);

        return new UploadImageResponseDto
        {
            Id = image.Id,
            FileName = image.FileName,
            FileUrl = image.FileUrl,
            FileSize = image.FileSize
        };
    }
}
