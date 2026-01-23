using FluentValidation;

namespace FitFanShop.Application.Modules.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("New password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("New password must contain at least one lowercase letter.")
            .Matches(@"\d")
            .WithMessage("New password must contain at least one digit.")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must be different from current password.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
