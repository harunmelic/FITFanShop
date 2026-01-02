using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;

namespace FitFanShop.Domain.Entities.Discounts;

public sealed class DiscountProductEntity : BaseEntity
{
    public int DiscountId { get; set; }
    public int ProductId { get; set; }

    public DiscountEntity? Discount { get; set; }
    public ProductEntity? Product { get; set; }
}
