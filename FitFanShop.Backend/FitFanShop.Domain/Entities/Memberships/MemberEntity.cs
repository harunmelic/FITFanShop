using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Memberships;

/// <summary>
/// Represents a club member with membership details.
/// Membership is active based on EndDate only.
/// </summary>
public sealed class MemberEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    /// <summary>
    /// Membership start date
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Membership end date (null = lifetime membership)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Price paid for membership
    /// </summary>
    public decimal PricePaid { get; set; }

    /// <summary>
    /// Computed property - checks if membership is currently active based on EndDate
    /// </summary>
    [NotMapped]
    public bool IsActive => EndDate == null || EndDate.Value >= DateTime.UtcNow;

    /// <summary>
    /// Computed property - checks if membership has expired
    /// </summary>
    [NotMapped]
    public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.UtcNow;
}
