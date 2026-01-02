using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IAppDbContext _ctx;

    public UpdateOrderStatusCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _ctx.Orders
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .Include(o => o.Tickets)
                .ThenInclude(t => t.TicketType)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order with id {request.OrderId} not found.");

        var newStatus = await _ctx.OrderStatuses
            .FirstOrDefaultAsync(x => x.Name == request.Status, cancellationToken);

        if (newStatus == null)
            throw new FitFanShopBusinessRuleException("InvalidStatus", $"Order status '{request.Status}' not found in the database.");

        var oldStatus = order.Status?.Name;

        if (oldStatus == request.Status)
            throw new FitFanShopBusinessRuleException("SameStatus", $"Order is already in '{request.Status}' status.");

        if (oldStatus == "Cancelled")
            throw new FitFanShopBusinessRuleException("CannotChangeFromCancelled", "Cannot change status from 'Cancelled' to another status.");

        if (oldStatus == "Delivered")
            throw new FitFanShopBusinessRuleException("CannotChangeFromDelivered", "Cannot change status from 'Delivered' to another status.");

        if (oldStatus == "Confirmed" && request.Status == "Pending")
            throw new FitFanShopBusinessRuleException("CannotRevertToPending", "Cannot change status from 'Confirmed' back to 'Pending'.");

        if (request.Status == "Cancelled" && oldStatus != "Pending")
            throw new FitFanShopBusinessRuleException("CanOnlyCancelPending", "Only 'Pending' orders can be cancelled.");

        order.StatusId = newStatus.Id;

        if (request.Status == "Cancelled")
        {
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
        }

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
