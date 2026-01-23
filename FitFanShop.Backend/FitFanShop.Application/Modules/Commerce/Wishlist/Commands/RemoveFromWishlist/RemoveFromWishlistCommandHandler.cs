using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public RemoveFromWishlistCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var item = await _ctx.WishlistProducts
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
            throw new FitFanShopNotFoundException($"Wishlist item with id {request.ItemId} not found.");

        if (item.Wishlist?.UserId != userId.Value)
            throw new UnauthorizedAccessException("You can only remove items from your own wishlist.");

        _ctx.WishlistProducts.Remove(item);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
