using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Sales;

public sealed class PaymentEntity : BaseEntity
{
    public int OrderId { get; set; }
    public OrderEntity? Order { get; set; }
    public string PaymentMethod { get; set; } = "Mock";
    public decimal Amount { get; set; }
}
