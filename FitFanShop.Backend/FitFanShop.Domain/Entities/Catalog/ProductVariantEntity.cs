using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Catalog;

public class ProductVariantEntity : BaseEntity
{
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public string Size { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    [NotMapped]
    public bool IsInStock => StockQuantity > 0;
    [NotMapped]
    public bool IsOutOfStock => StockQuantity == 0;
    public ICollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();
    public ICollection<OrderItemEntity> OrderItems { get; private set; } = new List<OrderItemEntity>();
    public static class Constraints
    {
        public const int SizeMaxLength = 20;
        public const int SkuMaxLength = 50;
    }
}
