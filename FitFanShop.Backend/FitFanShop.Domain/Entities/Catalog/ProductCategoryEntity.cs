using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Catalog;

public sealed class ProductCategoryEntity : BaseEntity
{
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }
}
