using MediatR;

namespace FitFanShop.Application.Modules.Events.Commands.CreateEvent;

public class CreateEventCommand : IRequest<EventDetailsDto>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public required string Location { get; set; }
}
