using MediatR;
using System.Collections.Generic;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<int>
{
    public List<CreateOrderProductItemDto> Products { get; set; } = new();
    public List<CreateOrderTicketItemDto> Tickets { get; set; } = new();
}

public class CreateOrderProductItemDto
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderTicketItemDto
{
    public int TicketTypeId { get; set; }
    public int Quantity { get; set; }
}
