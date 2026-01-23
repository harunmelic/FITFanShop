using FluentValidation;

namespace FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.ProductVariantId.HasValue ^ x.TicketTypeId.HasValue)
            .WithMessage("Must specify either ProductVariantId OR TicketTypeId, not both or neither.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
