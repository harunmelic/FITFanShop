using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Users.Commands.UpdateUserEnabled;

public class UpdateUserEnabledCommandHandler : IRequestHandler<UpdateUserEnabledCommand>
{
    private readonly IAppDbContext _ctx;

    public UpdateUserEnabledCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(UpdateUserEnabledCommand request, CancellationToken cancellationToken)
    {
        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new FitFanShopNotFoundException($"User with id {request.UserId} not found.");

        user.IsEnabled = request.IsEnabled;
        user.ModifiedAtUtc = DateTime.UtcNow;

        if (!request.IsEnabled)
        {
            user.TokenVersion++;

            var refreshTokens = await _ctx.RefreshTokens
                .Where(rt => rt.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            _ctx.RefreshTokens.RemoveRange(refreshTokens);
        }

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
