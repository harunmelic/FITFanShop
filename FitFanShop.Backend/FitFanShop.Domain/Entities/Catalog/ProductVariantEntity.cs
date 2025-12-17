using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Sales;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Catalog;

/// <summary>
/// Represents a product variant (e.g., different sizes of a jersey).
/// Each variant has its own stock quantity.
/// </summary>
public class ProductVariantEntity : BaseEntity
{
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    /// <summary>
    /// Size or variant name (e.g., "S", "M", "L", "XL", "XXL", "One Size")
    /// </summary>
    public string Size { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity for this specific variant
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Stock Keeping Unit - unique identifier for inventory management
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Computed property - checks if variant is in stock
    /// </summary>
    [NotMapped]
    public bool IsInStock => StockQuantity > 0;

    /// <summary>
    /// Computed property - checks if variant is out of stock
    /// </summary>
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
