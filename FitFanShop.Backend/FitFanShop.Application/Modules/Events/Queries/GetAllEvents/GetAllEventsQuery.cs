using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.Events.Queries.GetAllEvents;

public class GetAllEventsQuery : IRequest<PageResult<EventDto>>
{
    public PageRequest Paging { get; set; } = new();
    public bool? UpcomingOnly { get; set; }
}
