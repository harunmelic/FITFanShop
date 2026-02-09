namespace FitFanShop.Application.Modules.Media.Images;

public class UploadImageResponseDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
