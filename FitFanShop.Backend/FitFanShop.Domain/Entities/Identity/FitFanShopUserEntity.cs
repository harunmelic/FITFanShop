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
    public int TokenVersion { get; set; }
    public string? SecurityQuestion { get; set; }
    public string? SecurityAnswerHash { get; set; }
    public bool IsEnabled { get; set; } = true;
    public RoleEntity? Role { get; set; }
    public MemberEntity? MemberProfile { get; set; }
    public WishlistEntity? Wishlist { get; set; }
    public CartEntity? Cart { get; set; }
    public ICollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    public ICollection<ReviewEntity> Reviews { get; private set; } = new List<ReviewEntity>();
    [NotMapped]
    public bool IsAdmin => RoleId == (int)RoleType.Admin;
    [NotMapped]
    public bool IsMember => MemberProfile != null &&
                            (MemberProfile.EndDate == null || MemberProfile.EndDate > DateTime.UtcNow);
}

