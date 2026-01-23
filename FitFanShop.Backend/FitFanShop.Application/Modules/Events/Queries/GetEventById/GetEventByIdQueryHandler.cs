using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Queries.GetEventById;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDetailsDto>
{
    private readonly IAppDbContext _ctx;

    public GetEventByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<EventDetailsDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _ctx.Events
            .Include(e => e.TicketTypes)
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (eventEntity == null)
            throw new FitFanShopNotFoundException($"Event with id {request.Id} not found.");

        return new EventDetailsDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            EventDate = eventEntity.EventDate,
            Location = eventEntity.Location,
            HasPassed = eventEntity.HasPassed,
            TicketsAvailable = eventEntity.TicketsAvailable,
            TicketTypes = eventEntity.TicketTypes
                .Where(tt => !tt.IsDeleted)
                .Select(tt => new TicketTypeDto
                {
                    Id = tt.Id,
                    Name = tt.Name,
                    Price = tt.Price,
                    TotalAvailable = tt.TotalAvailable,
                    Description = tt.Description,
                    IsAvailable = tt.IsAvailable
                })
                .ToList()
        };
    }
}
