using MediatR;

namespace FitFanShop.Application.Modules.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}
