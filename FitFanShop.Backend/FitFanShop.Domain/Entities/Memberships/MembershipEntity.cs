using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Memberships;

public sealed class MembershipEntity : BaseEntity
{
    public int MemberId { get; set; }
    public MemberEntity? Member { get; set; }

    public DateTime ActivationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
}
