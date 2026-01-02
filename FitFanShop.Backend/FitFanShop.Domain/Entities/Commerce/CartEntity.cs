using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Commerce;

public sealed class CartEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public ICollection<CartItemEntity> Items { get; private set; } = new List<CartItemEntity>();
}
