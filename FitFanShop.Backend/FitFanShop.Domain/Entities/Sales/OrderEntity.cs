using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Tickets;
using FitFanShop.Domain.Entities.Identity;

namespace FitFanShop.Domain.Entities.Sales;

public class OrderEntity : BaseEntity
{
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public OrderStatusEntity? Status { get; set; }
    public PaymentEntity? Payment { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItemEntity> Items { get; private set; } = new List<OrderItemEntity>();
    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
}


