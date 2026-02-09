using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Media;

public class ImageEntity : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadDateUtc { get; set; }

    public static class Constraints
    {
        public const int FileNameMaxLength = 255;
        public const int FileUrlMaxLength = 500;
        public const int MimeTypeMaxLength = 100;
        public const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    }
}
