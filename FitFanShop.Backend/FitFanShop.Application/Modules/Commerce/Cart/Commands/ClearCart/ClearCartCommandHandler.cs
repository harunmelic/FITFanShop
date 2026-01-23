using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public ClearCartCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var cart = await _ctx.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value && !c.IsDeleted, cancellationToken);

        if (cart != null)
        {
            _ctx.CartItems.RemoveRange(cart.Items);
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
