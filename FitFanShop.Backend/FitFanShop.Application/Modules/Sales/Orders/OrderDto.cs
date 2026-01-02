namespace FitFanShop.Application.Modules.Sales.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public List<OrderTicketDto> Tickets { get; set; } = new();
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantSize { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderTicketDto
{
    public int Id { get; set; }
    public int TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal PricePaid { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
}
