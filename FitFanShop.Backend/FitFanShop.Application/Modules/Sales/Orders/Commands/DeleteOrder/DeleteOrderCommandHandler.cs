using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IAppDbContext _ctx;

    public DeleteOrderCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _ctx.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order with id {request.OrderId} not found.");

        order.IsDeleted = true;

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
