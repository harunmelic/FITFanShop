using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommand : IRequest<WishlistDto>
{
    public required int ProductId { get; set; }
}
