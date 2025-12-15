using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Catalog;

public class CategoryEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; } = true;

    public ICollection<ProductCategoryEntity> ProductCategories { get; private set; } = new List<ProductCategoryEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 100;
    }
}
