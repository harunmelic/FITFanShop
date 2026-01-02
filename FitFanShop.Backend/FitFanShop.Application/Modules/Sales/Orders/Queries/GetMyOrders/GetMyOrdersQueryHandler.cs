using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, PageResult<OrderDto>>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetMyOrdersQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<PageResult<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var ordersQuery = _ctx.Orders
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                .ThenInclude(v => v.Product)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.TicketType)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.Event)
            .Where(o => o.UserId == userId.Value)
            .OrderByDescending(o => o.CreatedAtUtc);

        var mappedQuery = ordersQuery.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status != null ? order.Status.Name : "Unknown",
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAtUtc,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductVariant != null && i.ProductVariant.Product != null ? i.ProductVariant.Product.Name : "Unknown",
                VariantSize = i.ProductVariant != null ? i.ProductVariant.Size : "Unknown",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.UnitPrice * i.Quantity
            }).ToList(),
            Tickets = order.Tickets.Select(t => new OrderTicketDto
            {
                Id = t.Id,
                TicketTypeId = t.TicketTypeId,
                TicketTypeName = t.TicketType != null ? t.TicketType.Name : "Unknown",
                EventName = t.Event != null ? t.Event.Name : "Unknown",
                EventDate = t.Event != null ? t.Event.EventDate : DateTime.MinValue,
                PricePaid = t.PricePaid,
                QRCode = t.QRCode,
                SeatNumber = t.SeatNumber ?? "Unknown"
            }).ToList()
        });

        return await PageResult<OrderDto>.FromQueryableAsync(mappedQuery, request.Paging, cancellationToken);
    }
}
