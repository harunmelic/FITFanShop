using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Discounts;
using FitFanShop.Domain.Entities.Reviews;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Domain.Entities.Catalog;

public class ProductEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool Exclusive { get; set; }
    public ICollection<ProductVariantEntity> Variants { get; private set; } = new List<ProductVariantEntity>();
    public ICollection<ProductCategoryEntity> ProductCategories { get; private set; } = new List<ProductCategoryEntity>();
    public ICollection<ReviewEntity> Reviews { get; private set; } = new List<ReviewEntity>();
    public ICollection<WishlistProductEntity> WishlistProducts { get; private set; } = new List<WishlistProductEntity>();
    public ICollection<DiscountProductEntity> DiscountProducts { get; private set; } = new List<DiscountProductEntity>();
    public static class Constraints
    {
        public const int NameMaxLength = 150;
        public const int DescriptionMaxLength = 1000;
    }
    public void UpdateIsEnabledBasedOnStock()
    {
        IsEnabled = Variants.Any(v => v.StockQuantity > 0);
    }
}
