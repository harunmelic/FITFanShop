using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommand : IRequest
{
    public int ItemId { get; set; }
}
