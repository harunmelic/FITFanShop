using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, WishlistDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IMediator _mediator;

    public AddToWishlistCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser, IMediator mediator)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<WishlistDto> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var product = await _ctx.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

        if (product == null)
            throw new FitFanShopNotFoundException($"Product with id {request.ProductId} not found.");

        var wishlist = await _ctx.Wishlists
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

        var existingItem = await _ctx.WishlistProducts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i =>
                i.WishlistId == wishlist.Id &&
                i.ProductId == request.ProductId,
                cancellationToken);

        if (existingItem != null)
        {
            if (existingItem.IsDeleted)
            {
                existingItem.IsDeleted = false;
                existingItem.ModifiedAtUtc = DateTime.UtcNow;
            }
            else
            {
                throw new FitFanShopBusinessRuleException("DUPLICATE_WISHLIST_ITEM", "Product already in wishlist.");
            }
        }
        else
        {
            var newItem = new Domain.Entities.Commerce.WishlistProductEntity
            {
                WishlistId = wishlist.Id,
                ProductId = request.ProductId
            };
            _ctx.WishlistProducts.Add(newItem);
        }

        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetWishlist.GetWishlistQuery(), cancellationToken);
    }
}
