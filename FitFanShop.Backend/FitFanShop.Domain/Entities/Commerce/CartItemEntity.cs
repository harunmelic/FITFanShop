using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Tickets;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Commerce;

public sealed class CartItemEntity : BaseEntity
{
    public int CartId { get; set; }
    public CartEntity? Cart { get; set; }

    /// <summary>
    /// FK to ProductVariant (for products like jerseys, mugs, etc.)
    /// </summary>
    public int? ProductVariantId { get; set; }
    public ProductVariantEntity? ProductVariant { get; set; }

    /// <summary>
    /// FK to TicketType (for event tickets)
    /// </summary>
    public int? TicketTypeId { get; set; }
    public TicketTypeEntity? TicketType { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Computed property - checks if this cart item is a product
    /// </summary>
    [NotMapped]
    public bool IsProduct => ProductVariantId.HasValue;

    /// <summary>
    /// Computed property - checks if this cart item is a ticket
    /// </summary>
    [NotMapped]
    public bool IsTicket => TicketTypeId.HasValue;
}
