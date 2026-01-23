using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public RemoveCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var item = await _ctx.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
            throw new FitFanShopNotFoundException($"Cart item with id {request.ItemId} not found.");

        if (item.Cart?.UserId != userId.Value)
            throw new UnauthorizedAccessException("You can only remove items from your own cart.");

        _ctx.CartItems.Remove(item);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
