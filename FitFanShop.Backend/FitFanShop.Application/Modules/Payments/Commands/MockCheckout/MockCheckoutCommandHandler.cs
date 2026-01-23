using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;
using FitFanShop.Domain.Entities.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Payments.Commands.MockCheckout;

public class MockCheckoutCommandHandler : IRequestHandler<MockCheckoutCommand, MockCheckoutResultDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;
    private readonly IMediator _mediator;

    public MockCheckoutCommandHandler(
        IAppDbContext ctx,
        IAppCurrentUser currentUser,
        IMediator mediator)
    {
        _ctx = ctx;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<MockCheckoutResultDto> Handle(MockCheckoutCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated.");

        // 1. Dohvati Cart korisnika
        var cart = await _ctx.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .Include(c => c.Items)
                .ThenInclude(i => i.TicketType)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, ct);

        if (cart == null || !cart.Items.Any(i => !i.IsDeleted))
            throw new FitFanShopBusinessRuleException("EmptyCart", "Cart is empty.");

        var activeItems = cart.Items.Where(i => !i.IsDeleted).ToList();

        // 2. Kreiraj Order iz Cart-a
        var createOrderCommand = new CreateOrderCommand
        {
            Products = activeItems
                .Where(i => i.ProductVariantId.HasValue)
                .Select(i => new CreateOrderProductItemDto
                {
                    ProductVariantId = i.ProductVariantId!.Value,
                    Quantity = i.Quantity
                }).ToList(),
            Tickets = activeItems
                .Where(i => i.TicketTypeId.HasValue)
                .Select(i => new CreateOrderTicketItemDto
                {
                    TicketTypeId = i.TicketTypeId!.Value,
                    Quantity = i.Quantity
                }).ToList()
        };

        var orderId = await _mediator.Send(createOrderCommand, ct);

        // 3. Dohvati kreiran Order da vidimo TotalAmount
        var order = await _ctx.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order {orderId} not found.");

        // 4. Kreiraj Mock Payment
        var payment = new PaymentEntity
        {
            OrderId = orderId,
            PaymentMethod = "Mock",
            Amount = order.TotalAmount,
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Payments.Add(payment);

        // 5. Isprazni Cart (hard delete)
        _ctx.CartItems.RemoveRange(activeItems);

        await _ctx.SaveChangesAsync(ct);

        return new MockCheckoutResultDto
        {
            OrderId = orderId,
            PaymentId = payment.Id,
            TotalAmount = order.TotalAmount,
            Message = "Payment successful! Order created."
        };
    }
}