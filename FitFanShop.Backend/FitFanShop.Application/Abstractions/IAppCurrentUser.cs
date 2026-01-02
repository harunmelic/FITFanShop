namespace FitFanShop.Application.Abstractions;
public interface IAppCurrentUser
{
    int? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    bool IsManager { get; }
    bool IsEmployee { get; }
}