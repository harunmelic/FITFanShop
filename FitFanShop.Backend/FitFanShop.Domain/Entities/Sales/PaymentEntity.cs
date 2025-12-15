using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Sales;

public sealed class PaymentEntity : BaseEntity
{
    public int OrderId { get; set; }
    public OrderEntity? Order { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
