using FluentValidation;
using FitFanShop.Domain.Entities.Tickets;

namespace FitFanShop.Application.Modules.Events.Commands.UpdateTicketType;

public class UpdateTicketTypeCommandValidator : AbstractValidator<UpdateTicketTypeCommand>
{
    public UpdateTicketTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Ticket type name is required.")
            .MaximumLength(TicketTypeEntity.Constraints.NameMaxLength)
            .WithMessage($"Ticket type name must not exceed {TicketTypeEntity.Constraints.NameMaxLength} characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Ticket price must be greater than 0.");

        RuleFor(x => x.TotalAvailable)
            .GreaterThan(0)
            .WithMessage("Total available tickets must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(TicketTypeEntity.Constraints.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage($"Ticket type description must not exceed {TicketTypeEntity.Constraints.DescriptionMaxLength} characters.");
    }
}
