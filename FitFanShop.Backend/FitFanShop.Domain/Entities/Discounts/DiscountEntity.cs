using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Discounts;

public sealed class DiscountEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }

    public ICollection<DiscountProductEntity> DiscountProducts { get; private set; } = new List<DiscountProductEntity>();
}
