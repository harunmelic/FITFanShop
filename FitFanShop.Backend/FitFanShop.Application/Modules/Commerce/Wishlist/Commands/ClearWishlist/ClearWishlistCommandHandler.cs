using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Commands.ClearWishlist;

public class ClearWishlistCommandHandler : IRequestHandler<ClearWishlistCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ClearWishlistCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(ClearWishlistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var wishlist = await _ctx.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && !w.IsDeleted, cancellationToken);

        if (wishlist != null)
        {
            _ctx.WishlistProducts.RemoveRange(wishlist.Items);
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
