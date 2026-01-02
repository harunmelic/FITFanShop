using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Notifications;

public sealed class NotificationEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime DateSent { get; set; } = DateTime.UtcNow;
}
