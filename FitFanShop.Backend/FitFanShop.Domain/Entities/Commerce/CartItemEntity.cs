using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;

namespace FitFanShop.Domain.Entities.Commerce;

public sealed class CartItemEntity : BaseEntity
{
    public int CartId { get; set; }
    public CartEntity? Cart { get; set; }

    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int Quantity { get; set; }
}
