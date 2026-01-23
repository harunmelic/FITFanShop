using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;

public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, CartDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IMediator _mediator;

    public AddCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser, IMediator mediator)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<CartDto> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var cart = await _ctx.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value && !c.IsDeleted, cancellationToken);

        if (cart == null)
        {
            cart = new Domain.Entities.Commerce.CartEntity
            {
                UserId = userId.Value
            };
            _ctx.Carts.Add(cart);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        if (request.ProductVariantId.HasValue)
        {
            var variant = await _ctx.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId.Value && !v.IsDeleted, cancellationToken);

            if (variant == null)
                throw new FitFanShopNotFoundException($"Product variant with id {request.ProductVariantId.Value} not found.");

            if (variant.StockQuantity < request.Quantity)
                throw new FitFanShopBusinessRuleException("INSUFFICIENT_STOCK", $"Only {variant.StockQuantity} items available in stock.");
        }

        if (request.TicketTypeId.HasValue)
        {
            var ticketType = await _ctx.TicketTypes
                .FirstOrDefaultAsync(t => t.Id == request.TicketTypeId.Value && !t.IsDeleted, cancellationToken);

            if (ticketType == null)
                throw new FitFanShopNotFoundException($"Ticket type with id {request.TicketTypeId.Value} not found.");

            if (ticketType.TotalAvailable < request.Quantity)
                throw new FitFanShopBusinessRuleException("INSUFFICIENT_TICKETS", $"Only {ticketType.TotalAvailable} tickets available.");
        }

        var existingItem = cart.Items.FirstOrDefault(i =>
            !i.IsDeleted &&
            i.ProductVariantId == request.ProductVariantId &&
            i.TicketTypeId == request.TicketTypeId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            var newItem = new Domain.Entities.Commerce.CartItemEntity
            {
                CartId = cart.Id,
                ProductVariantId = request.ProductVariantId,
                TicketTypeId = request.TicketTypeId,
                Quantity = request.Quantity
            };
            _ctx.CartItems.Add(newItem);
        }

        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetCart.GetCartQuery(), cancellationToken);
    }
}
