using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public class GetMyOrdersQuery : BasePagedQuery<OrderDto>, IRequest<PageResult<OrderDto>>
{
}
