using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Commerce;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Tickets;

/// <summary>
/// Represents a ticket type for an event (e.g., VIP, North, South, East, West sections).
/// Each type has its own price and stock availability.
/// </summary>
public sealed class TicketTypeEntity : BaseEntity
{
    public int EventId { get; set; }
    public EventEntity? Event { get; set; }

    /// <summary>
    /// Name of the ticket type (e.g., "VIP Lounge", "North Stand", "South Stand")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price for this ticket type
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Total number of tickets available for this type
    /// </summary>
    public int TotalAvailable { get; set; }

    /// <summary>
    /// Optional description of the ticket type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Computed property - number of tickets sold (excluding refunded)
    /// </summary>
    [NotMapped]
    public int Sold => Tickets.Count(t => t.Status != "Refunded");

    /// <summary>
    /// Computed property - remaining stock
    /// </summary>
    [NotMapped]
    public int RemainingStock => TotalAvailable - Sold;

    /// <summary>
    /// Computed property - checks if tickets are available for purchase
    /// </summary>
    [NotMapped]
    public bool IsAvailable => RemainingStock > 0 && Event?.TicketsAvailable == true;

    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
    public ICollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;
    }
}
