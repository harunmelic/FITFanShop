using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Commerce;

public sealed class WishlistEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public ICollection<WishlistProductEntity> Items { get; private set; } = new List<WishlistProductEntity>();
}
