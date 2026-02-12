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

        // 1. Get user's cart
        var cart = await _ctx.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
            .Include(c => c.Items)
                .ThenInclude(i => i.TicketType)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, ct);

        if (cart == null || !cart.Items.Any(i => !i.IsDeleted))
            throw new FitFanShopBusinessRuleException("EmptyCart", "Cart is empty.");

        var activeItems = cart.Items.Where(i => !i.IsDeleted).ToList();

        // 2. Validation: Check if all ProductVariants exist in the database
        var productVariantIds = activeItems
            .Where(i => i.ProductVariantId.HasValue)
            .Select(i => i.ProductVariantId!.Value)
            .ToList();

        if (productVariantIds.Any())
        {
            var existingVariantIds = await _ctx.ProductVariants
                .Where(v => productVariantIds.Contains(v.Id) && !v.IsDeleted)
                .Select(v => v.Id)
                .ToListAsync(ct);

            var missingVariantIds = productVariantIds.Except(existingVariantIds).ToList();
            if (missingVariantIds.Any())
            {
                throw new FitFanShopBusinessRuleException(
                    "InvalidCartItems",
                    $"Some products in your cart are no longer available. Please remove them and try again. Missing variant IDs: {string.Join(", ", missingVariantIds)}");
            }
        }

        // 3. Create Order from Cart
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

        var orderResponse = await _mediator.Send(createOrderCommand, ct);

        // 4. Fetch created Order to get TotalAmount
        var order = await _ctx.Orders
            .FirstOrDefaultAsync(o => o.Id == orderResponse.OrderId, ct);

        if (order == null)
            throw new FitFanShopNotFoundException($"Order {orderResponse.OrderId} not found.");

        // 5. Create Mock Payment
        var payment = new PaymentEntity
        {
            OrderId = orderResponse.OrderId,
            PaymentMethod = "Mock",
            Amount = order.TotalAmount,
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Payments.Add(payment);

        // 6. Automatically change status to "Confirmed" after successful payment
        var confirmedStatus = await _ctx.OrderStatuses
            .FirstOrDefaultAsync(s => s.Name == "Confirmed", ct);

        if (confirmedStatus != null)
        {
            order.StatusId = confirmedStatus.Id;
            order.ModifiedAtUtc = DateTime.UtcNow;
        }

        // 7. Empty Cart (hard delete)
        _ctx.CartItems.RemoveRange(activeItems);

        await _ctx.SaveChangesAsync(ct);

        return new MockCheckoutResultDto
        {
            OrderId = orderResponse.OrderId,
            PaymentId = payment.Id,
            TotalAmount = order.TotalAmount,
            Message = "Payment successful! Order created."
        };
    }
}