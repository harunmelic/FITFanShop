using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Queries.GetWishlist;

public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, WishlistDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetWishlistQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<WishlistDto> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var wishlist = await _ctx.Wishlists
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Variants)
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && !w.IsDeleted, cancellationToken);

        if (wishlist == null)
        {
            wishlist = new Domain.Entities.Commerce.WishlistEntity
            {
                UserId = userId.Value
            };
            _ctx.Wishlists.Add(wishlist);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        var items = wishlist.Items
            .Where(i => !i.IsDeleted)
            .Select(i => new WishlistItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                Price = i.Product?.Price ?? 0,
                ImageUrl = i.Product?.Image,
                IsAvailable = i.Product != null && i.Product.Variants.Any(v => v.StockQuantity > 0),
                IsExclusive = i.Product?.Exclusive ?? false,
                AddedAt = i.CreatedAtUtc
            })
            .ToList();

        return new WishlistDto
        {
            Id = wishlist.Id,
            UserId = wishlist.UserId,
            Items = items,
            ItemCount = items.Count
        };
    }
}
