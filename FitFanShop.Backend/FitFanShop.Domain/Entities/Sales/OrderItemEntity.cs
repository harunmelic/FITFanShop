using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Reviews;

namespace FitFanShop.Domain.Entities.Sales;

public class OrderItemEntity : BaseEntity
{
    public int OrderId { get; set; }
    public OrderEntity? Order { get; set; }

    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public ReviewEntity? Review { get; set; }
}
