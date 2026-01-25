using FluentValidation;

namespace FitFanShop.Application.Modules.Auth.Commands.VerifySecurityAnswer;

public class VerifySecurityAnswerCommandValidator : AbstractValidator<VerifySecurityAnswerCommand>
{
    public VerifySecurityAnswerCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.SecurityAnswer)
            .NotEmpty()
            .WithMessage("Security answer is required.");
    }
}
