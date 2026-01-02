namespace FitFanShop.Application.Modules.Auth.Commands.Login;
public sealed class LoginCommandDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}