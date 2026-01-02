using MediatR;

namespace FitFanShop.Application.Modules.Sales.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public int Id { get; set; }
}
