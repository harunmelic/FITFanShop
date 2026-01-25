using MediatR;

namespace FitFanShop.Application.Modules.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<ResetPasswordDto>
{
    public required string Email { get; init; }
    public required string ResetToken { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmNewPassword { get; init; }
}

public record ResetPasswordDto(bool Success, string Message);
