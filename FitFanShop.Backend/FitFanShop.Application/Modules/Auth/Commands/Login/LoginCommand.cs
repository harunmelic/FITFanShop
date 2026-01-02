namespace FitFanShop.Application.Modules.Auth.Commands.Login;
public sealed class LoginCommand : IRequest<LoginCommandDto>
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string? Fingerprint { get; init; }
}