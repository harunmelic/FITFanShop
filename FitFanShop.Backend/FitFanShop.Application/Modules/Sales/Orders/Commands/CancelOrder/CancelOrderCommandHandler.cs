using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CancelOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var order = await _ctx.Orders
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.TicketType)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order with id {request.OrderId} not found.");

        if (order.UserId != userId.Value)
            throw new UnauthorizedAccessException("You can only cancel your own orders.");

        if (order.Status?.Name != "Pending")
            throw new FitFanShopBusinessRuleException("CannotCancel", "Only orders with 'Pending' status can be cancelled.");

        var cancelledStatus = await _ctx.OrderStatuses.FirstOrDefaultAsync(x => x.Name == "Cancelled", cancellationToken);
        if (cancelledStatus == null)
            throw new FitFanShopBusinessRuleException("StatusNotFound", "Order status 'Cancelled' not found in the database.");

        order.StatusId = cancelledStatus.Id;

        foreach (var item in order.Items)
        {
            if (item.ProductVariant != null)
                item.ProductVariant.StockQuantity += item.Quantity;
        }

        foreach (var ticket in order.Tickets)
        {
            if (ticket.TicketType != null)
                ticket.TicketType.TotalAvailable += 1;
        }

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
