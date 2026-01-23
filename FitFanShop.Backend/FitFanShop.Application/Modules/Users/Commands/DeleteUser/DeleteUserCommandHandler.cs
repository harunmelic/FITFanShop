using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public DeleteUserCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.UserId;
        if (currentUserId == null)
            throw new UnauthorizedAccessException();

        if (currentUserId.Value == request.UserId)
            throw new FitFanShopBusinessRuleException("CANNOT_DELETE_SELF", "Admin cannot delete their own account.");

        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new FitFanShopNotFoundException($"User with id {request.UserId} not found.");

        user.IsDeleted = true;
        user.TokenVersion++;
        user.ModifiedAtUtc = DateTime.UtcNow;

        var refreshTokens = await _ctx.RefreshTokens
            .Where(rt => rt.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        _ctx.RefreshTokens.RemoveRange(refreshTokens);

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
