using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IMediator _mediator;

    public UpdateCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser, IMediator mediator)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var item = await _ctx.CartItems
            .Include(i => i.Cart)
            .Include(i => i.ProductVariant)
            .Include(i => i.TicketType)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
            throw new FitFanShopNotFoundException($"Cart item with id {request.ItemId} not found.");

        if (item.Cart?.UserId != userId.Value)
            throw new UnauthorizedAccessException("You can only update items in your own cart.");

        if (request.Quantity == 0)
        {
            _ctx.CartItems.Remove(item);
            await _ctx.SaveChangesAsync(cancellationToken);
            return await _mediator.Send(new Queries.GetCart.GetCartQuery(), cancellationToken);
        }

        if (item.ProductVariantId.HasValue && item.ProductVariant != null)
        {
            if (item.ProductVariant.StockQuantity < request.Quantity)
                throw new FitFanShopBusinessRuleException("INSUFFICIENT_STOCK", $"Only {item.ProductVariant.StockQuantity} items available in stock.");
        }

        if (item.TicketTypeId.HasValue && item.TicketType != null)
        {
            if (item.TicketType.TotalAvailable < request.Quantity)
                throw new FitFanShopBusinessRuleException("INSUFFICIENT_TICKETS", $"Only {item.TicketType.TotalAvailable} tickets available.");
        }

        item.Quantity = request.Quantity;
        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetCart.GetCartQuery(), cancellationToken);
    }
}
