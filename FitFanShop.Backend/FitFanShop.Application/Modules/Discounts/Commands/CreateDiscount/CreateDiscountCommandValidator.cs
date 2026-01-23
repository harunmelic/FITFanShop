using FluentValidation;

namespace FitFanShop.Application.Modules.Discounts.Commands.CreateDiscount;

public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Discount name is required.")
            .MaximumLength(200)
            .WithMessage("Discount name must not exceed 200 characters.");

        RuleFor(x => x.Percentage)
            .GreaterThan(0)
            .WithMessage("Discount percentage must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100%.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date.");
    }
}
