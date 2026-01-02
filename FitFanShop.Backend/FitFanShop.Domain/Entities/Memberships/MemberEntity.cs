using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Memberships;

public sealed class MemberEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public decimal PricePaid { get; set; }
    [NotMapped]
    public bool IsActive => EndDate == null || EndDate.Value >= DateTime.UtcNow;
    [NotMapped]
    public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.UtcNow;
}

