using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetOrderByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var order = await _ctx.Orders
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                .ThenInclude(v => v.Product)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.TicketType)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.Event)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order with id {request.Id} not found.");

        if (order.UserId != userId.Value)
            throw new UnauthorizedAccessException("You can only view your own orders.");

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status?.Name ?? "Unknown",
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAtUtc,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductVariant?.Product?.Name ?? "Unknown",
                VariantSize = i.ProductVariant?.Size ?? "Unknown",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.UnitPrice * i.Quantity
            }).ToList(),
            Tickets = order.Tickets.Select(t => new OrderTicketDto
            {
                Id = t.Id,
                TicketTypeId = t.TicketTypeId,
                TicketTypeName = t.TicketType?.Name ?? "Unknown",
                EventName = t.Event?.Name ?? "Unknown",
                EventDate = t.Event?.EventDate ?? DateTime.MinValue,
                PricePaid = t.PricePaid,
                QRCode = t.QRCode,
                SeatNumber = t.SeatNumber ?? "Unknown"
            }).ToList()
        };
    }
}
