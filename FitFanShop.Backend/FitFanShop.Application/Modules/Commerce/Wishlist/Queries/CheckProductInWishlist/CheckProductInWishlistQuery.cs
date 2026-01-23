using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Wishlist.Queries.CheckProductInWishlist;

public class CheckProductInWishlistQuery : IRequest<CheckProductInWishlistDto>
{
    public int ProductId { get; set; }
}
