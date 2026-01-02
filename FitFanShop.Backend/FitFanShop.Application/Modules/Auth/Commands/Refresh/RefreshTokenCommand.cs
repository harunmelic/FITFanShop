namespace FitFanShop.Application.Modules.Auth.Commands.Refresh;
public sealed class RefreshTokenCommand : IRequest<RefreshTokenCommandDto>
{
    public string RefreshToken { get; init; }
    public string? Fingerprint { get; init; }
}