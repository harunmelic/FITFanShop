namespace FitFanShop.Application.Modules.Auth.Commands.Refresh;
public sealed class RefreshTokenCommandDto
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime AccessTokenExpiresAtUtc { get; init; }
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
}