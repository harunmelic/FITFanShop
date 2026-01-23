using MediatR;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;

public class AddCartItemCommand : IRequest<CartDto>
{
    public int? ProductVariantId { get; set; }
    public int? TicketTypeId { get; set; }
    public int Quantity { get; set; }
}
