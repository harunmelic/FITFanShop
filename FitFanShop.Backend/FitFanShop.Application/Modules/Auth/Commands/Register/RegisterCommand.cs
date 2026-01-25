using FitFanShop.Application.Modules.Auth.Commands.Login;
using MediatR;

namespace FitFanShop.Application.Modules.Auth.Commands.Register;

public class RegisterCommand : IRequest<LoginCommandDto>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string SecurityQuestion { get; set; }
    public required string SecurityAnswer { get; set; }
}

