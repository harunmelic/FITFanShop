using FluentValidation;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description)
            .NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price)
            .GreaterThan(0);
        RuleFor(x => x.CategoryIds)
            .Must(x => x == null || x.Count > 0)
            .WithMessage("If categories are sent, at least one category is required.");
    }
}
