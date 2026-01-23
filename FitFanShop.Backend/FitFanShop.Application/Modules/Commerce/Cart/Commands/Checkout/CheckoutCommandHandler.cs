using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.Checkout;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, int>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IMediator _mediator;

    public CheckoutCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser, IMediator mediator)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<int> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var cart = await _ctx.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .Include(c => c.Items)
                .ThenInclude(i => i.TicketType)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value && !c.IsDeleted, cancellationToken);

        if (cart == null || !cart.Items.Any(i => !i.IsDeleted))
            throw new FitFanShopBusinessRuleException("EMPTY_CART", "Cart is empty. Cannot proceed with checkout.");

        var activeItems = cart.Items.Where(i => !i.IsDeleted).ToList();

        var products = activeItems
            .Where(i => i.ProductVariantId.HasValue)
            .Select(i => new CreateOrderProductItemDto
            {
                ProductVariantId = i.ProductVariantId!.Value,
                Quantity = i.Quantity
            })
            .ToList();

        var tickets = activeItems
            .Where(i => i.TicketTypeId.HasValue)
            .Select(i => new CreateOrderTicketItemDto
            {
                TicketTypeId = i.TicketTypeId!.Value,
                Quantity = i.Quantity
            })
            .ToList();

        var createOrderCommand = new CreateOrderCommand
        {
            Products = products,
            Tickets = tickets
        };

        var orderId = await _mediator.Send(createOrderCommand, cancellationToken);

        _ctx.CartItems.RemoveRange(activeItems);
        await _ctx.SaveChangesAsync(cancellationToken);

        return orderId;
    }
}
