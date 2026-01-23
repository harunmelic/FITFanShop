using MediatR;

namespace FitFanShop.Application.Modules.Events.Queries.GetUpcomingEvents;

public class GetUpcomingEventsQuery : IRequest<List<EventDto>>
{
}
