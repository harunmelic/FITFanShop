using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Domain.Entities.Tickets;

public sealed class TicketEntity : BaseEntity
{
    public int EventId { get; set; }
    public EventEntity? Event { get; set; }

    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool Presale { get; set; }

    public int? OrderId { get; set; }
    public OrderEntity? Order { get; set; }
}
