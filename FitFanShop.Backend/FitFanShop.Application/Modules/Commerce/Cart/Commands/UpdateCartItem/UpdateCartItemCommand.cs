using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommand : IRequest<CartDto>
{
    public int ItemId { get; set; }
    public required int Quantity { get; set; }
}
