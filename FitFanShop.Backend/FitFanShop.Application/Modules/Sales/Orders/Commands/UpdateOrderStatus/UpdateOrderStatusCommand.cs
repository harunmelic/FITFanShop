using MediatR;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest
{
    public int OrderId { get; set; }
    public required string Status { get; set; }
}
