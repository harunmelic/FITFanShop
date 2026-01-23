using MediatR;

namespace FitFanShop.Application.Modules.Events.Queries.GetEventById;

public class GetEventByIdQuery : IRequest<EventDetailsDto>
{
    public int Id { get; set; }
}
