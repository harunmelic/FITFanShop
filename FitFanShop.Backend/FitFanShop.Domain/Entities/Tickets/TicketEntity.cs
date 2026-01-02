using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Domain.Entities.Tickets;

public sealed class TicketEntity : BaseEntity
{
    public int TicketTypeId { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public TicketTypeEntity? TicketType { get; set; }
    public EventEntity? Event { get; set; }
    public FitFanShopUserEntity? User { get; set; }
    public OrderEntity? Order { get; set; }
    public decimal PricePaid { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public string? SeatNumber { get; set; }
    public static class Constraints
    {
        public const int QRCodeMaxLength = 100;
        public const int SeatNumberMaxLength = 20;
        public const int StatusMaxLength = 20;
    }
}
