using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public GetCartQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var cart = await _ctx.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.TicketType)
                    .ThenInclude(tt => tt.Event)
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

        var items = cart.Items
            .Where(i => !i.IsDeleted)
            .Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductVariant?.Product?.Name,
                Size = i.ProductVariant?.Size,
                ImageUrl = i.ProductVariant?.Product?.Image,
                InStock = i.ProductVariant != null && !i.ProductVariant.IsDeleted && i.ProductVariant.StockQuantity >= i.Quantity,
                TicketTypeId = i.TicketTypeId,
                TicketTypeName = i.TicketType?.Name,
                EventName = i.TicketType?.Event?.Name,
                EventDate = i.TicketType?.Event?.EventDate,
                Available = i.TicketType != null && !i.TicketType.IsDeleted && i.TicketType.TotalAvailable >= i.Quantity,
                UnitPrice = i.ProductVariant?.Product?.Price ?? i.TicketType?.Price ?? 0,
                Quantity = i.Quantity,
                TotalPrice = (i.ProductVariant?.Product?.Price ?? i.TicketType?.Price ?? 0) * i.Quantity,
                IsProduct = i.ProductVariantId.HasValue,
                IsTicket = i.TicketTypeId.HasValue
            })
            .ToList();

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = items,
            TotalAmount = items.Sum(i => i.TotalPrice),
            ItemCount = items.Sum(i => i.Quantity)
        };
    }
}
