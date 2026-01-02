using FitFanShop.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Discounts;

public sealed class DiscountEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool MembersOnly { get; set; }
    [NotMapped]
    public bool IsActive => StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;
    public ICollection<DiscountProductEntity> DiscountProducts { get; private set; } = new List<DiscountProductEntity>();
}

