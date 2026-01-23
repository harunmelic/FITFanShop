using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommand : IRequest
{
    public int ItemId { get; set; }
}
