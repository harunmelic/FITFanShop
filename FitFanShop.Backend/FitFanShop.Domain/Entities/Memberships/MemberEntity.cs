using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Memberships;

public sealed class MemberEntity : BaseEntity
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public string MembershipStatus { get; set; } = string.Empty;

    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    public ICollection<MembershipEntity> Memberships { get; private set; } = new List<MembershipEntity>();
}
