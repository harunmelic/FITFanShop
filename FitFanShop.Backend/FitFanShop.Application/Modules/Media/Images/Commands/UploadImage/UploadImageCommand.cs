using MediatR;

namespace FitFanShop.Application.Modules.Media.Images.Commands.UploadImage;

public class UploadImageCommand : IRequest<UploadImageResponseDto>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
