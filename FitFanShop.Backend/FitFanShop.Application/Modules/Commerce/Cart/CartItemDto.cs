namespace FitFanShop.Application.Modules.Commerce.Cart;

public class CartItemDto
{
    public int Id { get; set; }

    public int? ProductVariantId { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Size { get; set; }
    public string? ImageUrl { get; set; }
    public bool InStock { get; set; }

    public int? TicketTypeId { get; set; }
    public string? TicketTypeName { get; set; }
    public string? EventName { get; set; }
    public DateTime? EventDate { get; set; }
    public bool Available { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsProduct { get; set; }
    public bool IsTicket { get; set; }
}
