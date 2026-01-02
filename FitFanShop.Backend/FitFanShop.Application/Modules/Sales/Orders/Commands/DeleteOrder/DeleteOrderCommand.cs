using MediatR;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest
{
    public int OrderId { get; set; }
}
