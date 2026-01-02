namespace FitFanShop.Application.Abstractions;
public sealed class JwtTokenPair
{
    public string AccessToken { get; init; }
    public DateTime AccessTokenExpiresAtUtc { get; init; }
    public string RefreshTokenRaw { get; init; }
    public string RefreshTokenHash { get; init; }
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
}
public interface IJwtTokenService
{
    JwtTokenPair IssueTokens(FitFanShopUserEntity user);
    string HashRefreshToken(string rawToken);
}