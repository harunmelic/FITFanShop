namespace FitFanShop.Application.Modules.Auth.Commands.Logout;
public sealed class LogoutCommandHandler(IAppDbContext ctx, IJwtTokenService tokens) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var hash = tokens.HashRefreshToken(request.RefreshToken);
        var rt = await ctx.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == hash &&
                !x.IsRevoked &&
                !x.IsDeleted, ct);
        if (rt is null)
            return;
        rt.IsRevoked = true;
        await ctx.SaveChangesAsync(ct);
    }
}