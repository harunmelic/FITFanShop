using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Sales;

namespace FitFanShop.Domain.Entities.Tickets;

/// <summary>
/// Represents an individual purchased ticket.
/// Each ticket has a unique QR code for check-in at the event.
/// </summary>
public sealed class TicketEntity : BaseEntity
{
    public int TicketTypeId { get; set; }
    public TicketTypeEntity? TicketType { get; set; }

    public int EventId { get; set; }
    public EventEntity? Event { get; set; }

    public int UserId { get; set; }
    public FitFanShopUserEntity? User { get; set; }

    public int? OrderId { get; set; }
    public OrderEntity? Order { get; set; }

    /// <summary>
    /// Price paid for this ticket
    /// </summary>
    public decimal PricePaid { get; set; }

    /// <summary>
    /// Unique QR code for check-in
    /// </summary>
    public string QRCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional seat number (e.g., "N-15-A")
    /// </summary>
    public string? SeatNumber { get; set; }

    /// <summary>
    /// Ticket status: Valid, Used, Cancelled, Refunded
    /// </summary>
    public string Status { get; set; } = "Valid";

    /// <summary>
    /// When the ticket was purchased
    /// </summary>
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the ticket was used (scanned at entrance)
    /// </summary>
    public DateTime? UsedAtUtc { get; set; }

    public static class Constraints
    {
        public const int QRCodeMaxLength = 100;
        public const int SeatNumberMaxLength = 20;
        public const int StatusMaxLength = 20;
    }
}
