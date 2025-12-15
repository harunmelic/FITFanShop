using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Domain.Entities.Reviews;

public sealed class ReviewEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;

    public int OrderItemId { get; set; }
    public OrderItemEntity? OrderItem { get; set; }
}
