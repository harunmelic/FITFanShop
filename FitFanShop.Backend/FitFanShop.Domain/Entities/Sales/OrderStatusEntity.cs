using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Sales;

public sealed class OrderStatusEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
}
