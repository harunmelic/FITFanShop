using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Queries.CheckProductInWishlist;

public class CheckProductInWishlistQueryHandler : IRequestHandler<CheckProductInWishlistQuery, CheckProductInWishlistDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CheckProductInWishlistQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<CheckProductInWishlistDto> Handle(CheckProductInWishlistQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var wishlist = await _ctx.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && !w.IsDeleted, cancellationToken);

        if (wishlist == null)
        {
            return new CheckProductInWishlistDto
            {
                IsInWishlist = false,
                ItemId = null
            };
        }

        var item = wishlist.Items.FirstOrDefault(i => 
            i.ProductId == request.ProductId && !i.IsDeleted);

        return new CheckProductInWishlistDto
        {
            IsInWishlist = item != null,
            ItemId = item?.Id
        };
    }
}
