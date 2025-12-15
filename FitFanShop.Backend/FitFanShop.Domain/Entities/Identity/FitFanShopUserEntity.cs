using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Memberships;
using FitFanShop.Domain.Entities.Notifications;
using FitFanShop.Domain.Entities.Reviews;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Domain.Entities.Tickets;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Identity;

public sealed class FitFanShopUserEntity : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public int RoleId { get; set; } = (int)RoleType.User;
    public RoleEntity? Role { get; set; }

    public int TokenVersion { get; set; }
    public bool IsEnabled { get; set; } = true;

    // Computed properties - not stored in database
    [NotMapped]
    public bool IsAdmin => RoleId == (int)RoleType.Admin;

    [NotMapped]
    public bool IsMember => MemberProfile != null &&
                            (MemberProfile.EndDate == null || MemberProfile.EndDate > DateTime.UtcNow);

    public MemberEntity? MemberProfile { get; set; }
    public WishlistEntity? Wishlist { get; set; }
    public CartEntity? Cart { get; set; }

    public ICollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    public ICollection<ReviewEntity> Reviews { get; private set; } = new List<ReviewEntity>();
    public ICollection<NotificationEntity> Notifications { get; private set; } = new List<NotificationEntity>();
    public ICollection<ActivityLogEntity> ActivityLogs { get; private set; } = new List<ActivityLogEntity>();
    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}
