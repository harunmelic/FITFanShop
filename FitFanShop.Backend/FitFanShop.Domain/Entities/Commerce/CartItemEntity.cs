using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Tickets;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Commerce;

public sealed class CartItemEntity : BaseEntity
{
    public int CartId { get; set; }
    public CartEntity? Cart { get; set; }
    public int? ProductVariantId { get; set; }
    public ProductVariantEntity? ProductVariant { get; set; }
    public int? TicketTypeId { get; set; }
    public TicketTypeEntity? TicketType { get; set; }
    public int Quantity { get; set; }
    [NotMapped]
    public bool IsProduct => ProductVariantId.HasValue;
    [NotMapped]
    public bool IsTicket => TicketTypeId.HasValue;
}
