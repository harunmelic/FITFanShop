using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Queries.GetUpcomingEvents;

public class GetUpcomingEventsQueryHandler : IRequestHandler<GetUpcomingEventsQuery, List<EventDto>>
{
    private readonly IAppDbContext _ctx;

    public GetUpcomingEventsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<EventDto>> Handle(GetUpcomingEventsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _ctx.Events
            .Include(e => e.TicketTypes)
            .Where(e => !e.IsDeleted && e.EventDate >= now)
            .OrderBy(e => e.EventDate)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                EventDate = e.EventDate,
                Location = e.Location,
                HasPassed = false,
                TicketsAvailable = e.TicketsAvailable,
                TotalTicketTypes = e.TicketTypes.Count
            })
            .ToListAsync(cancellationToken);
    }
}
