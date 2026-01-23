using FluentValidation;
using FitFanShop.Domain.Entities.Tickets;

namespace FitFanShop.Application.Modules.Events.Commands.UpdateEvent;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Event name is required.")
            .MaximumLength(EventEntity.Constraints.NameMaxLength)
            .WithMessage($"Event name must not exceed {EventEntity.Constraints.NameMaxLength} characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Event location is required.")
            .MaximumLength(EventEntity.Constraints.LocationMaxLength)
            .WithMessage($"Event location must not exceed {EventEntity.Constraints.LocationMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(EventEntity.Constraints.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage($"Event description must not exceed {EventEntity.Constraints.DescriptionMaxLength} characters.");

        RuleFor(x => x.EventDate)
            .NotEmpty()
            .WithMessage("Event date is required.");
    }
}
