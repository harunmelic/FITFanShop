using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Tickets;

namespace FitFanShop.Domain.Entities.Sales;

public class OrderEntity : BaseEntity
{
    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public int StatusId { get; set; }
    public OrderStatusEntity? Status { get; set; }

    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }

    public PaymentEntity? Payment { get; set; }

    public ICollection<OrderItemEntity> Items { get; private set; } = new List<OrderItemEntity>();
    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
}


