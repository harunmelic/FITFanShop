using MediatR;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest
{
    public int OrderId { get; set; }
}
