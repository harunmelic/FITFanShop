using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt,
    IPasswordHasher<FitFanShopUserEntity> hasher)
    : IRequestHandler<LoginCommand, LoginCommandDto>
{
    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.IsEnabled && !x.IsDeleted, ct)
            ?? throw new FitFanShopNotFoundException("User not found or disabled.");

        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new FitFanShopInvalidCredentialsException("Invalid credentials.");

        var tokens = jwt.IssueTokens(user);

        ctx.RefreshTokens.Add(new RefreshTokenEntity
        {
            TokenHash = tokens.RefreshTokenHash,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc,
            UserId = user.Id,
            Fingerprint = request.Fingerprint
        });

        await ctx.SaveChangesAsync(ct);

        return new LoginCommandDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshTokenRaw,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };
    }
}
