using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Queries.GetAllEvents;

public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, PageResult<EventDto>>
{
    private readonly IAppDbContext _ctx;

    public GetAllEventsQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PageResult<EventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _ctx.Events
            .Include(e => e.TicketTypes)
            .Where(e => !e.IsDeleted)
            .AsQueryable();

        var now = DateTime.UtcNow;

        if (request.UpcomingOnly == true)
        {
            query = query.Where(e => e.EventDate >= now);
        }
        else if (request.UpcomingOnly == false)
        {
            query = query.Where(e => e.EventDate < now);
        }
        // If upcomingOnly is null, no filter is applied (returns all events)

        var total = await query.CountAsync(cancellationToken);

        var events = await query
            .OrderBy(e => e.EventDate)
            .Skip((request.Paging.Page - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                EventDate = e.EventDate,
                Location = e.Location,
                HasPassed = e.HasPassed,
                TicketsAvailable = e.TicketsAvailable,
                TotalTicketTypes = e.TicketTypes.Count
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.Paging.PageSize);

        return new PageResult<EventDto>
        {
            Items = events,
            TotalItems = total,
            CurrentPage = request.Paging.Page,
            PageSize = request.Paging.PageSize,
            IncludedTotal = true,
            TotalPages = totalPages
        };
    }
}
