using FluentValidation;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description)
            .NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price)
            .GreaterThan(0);
        RuleFor(x => x.CategoryIds)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage("At least one category is required.");
        RuleFor(x => x.Variants)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage("At least one product variant is required.");
        RuleForEach(x => x.Variants).SetValidator(new CreateProductVariantDtoValidator());
    }
}

public class CreateProductVariantDtoValidator : AbstractValidator<CreateProductVariantDto>
{
    public CreateProductVariantDtoValidator()
    {
        RuleFor(x => x.Size).NotEmpty().MaximumLength(20);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
