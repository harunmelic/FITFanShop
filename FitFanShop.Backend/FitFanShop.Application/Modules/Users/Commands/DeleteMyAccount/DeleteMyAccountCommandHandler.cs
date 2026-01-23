using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Users.Commands.DeleteMyAccount;

public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteMyAccountCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new FitFanShopNotFoundException("User not found.");

        user.IsDeleted = true;
        user.TokenVersion++;
        user.ModifiedAtUtc = DateTime.UtcNow;

        var refreshTokens = await _ctx.RefreshTokens
            .Where(rt => rt.UserId == userId.Value)
            .ToListAsync(cancellationToken);

        _ctx.RefreshTokens.RemoveRange(refreshTokens);

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
