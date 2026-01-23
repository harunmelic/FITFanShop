using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Notifications;

public sealed class ActivityLogEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public string ActionDescription { get; set; } = string.Empty;
}
