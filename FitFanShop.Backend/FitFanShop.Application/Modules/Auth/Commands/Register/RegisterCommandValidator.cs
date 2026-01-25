using FluentValidation;

namespace FitFanShop.Application.Modules.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(x => x.SecurityQuestion)
            .NotEmpty()
            .WithMessage("Security question is required.")
            .MaximumLength(200)
            .WithMessage("Security question must not exceed 200 characters.");

        RuleFor(x => x.SecurityAnswer)
            .NotEmpty()
            .WithMessage("Security answer is required.")
            .MinimumLength(3)
            .WithMessage("Security answer must be at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Security answer must not exceed 100 characters.");
    }
}
