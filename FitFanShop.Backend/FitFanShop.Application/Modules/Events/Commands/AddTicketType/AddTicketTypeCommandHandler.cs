using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Tickets;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Commands.AddTicketType;

public class AddTicketTypeCommandHandler : IRequestHandler<AddTicketTypeCommand, TicketTypeDto>
{
    private readonly IAppDbContext _ctx;

    public AddTicketTypeCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<TicketTypeDto> Handle(AddTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _ctx.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId && !e.IsDeleted, cancellationToken);

        if (eventEntity == null)
            throw new FitFanShopNotFoundException($"Event with id {request.EventId} not found.");

        var ticketType = new TicketTypeEntity
        {
            EventId = request.EventId,
            Name = request.Name.Trim(),
            Price = request.Price,
            TotalAvailable = request.TotalAvailable,
            Description = request.Description?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.TicketTypes.Add(ticketType);
        await _ctx.SaveChangesAsync(cancellationToken);

        return new TicketTypeDto
        {
            Id = ticketType.Id,
            Name = ticketType.Name,
            Price = ticketType.Price,
            TotalAvailable = ticketType.TotalAvailable,
            Description = ticketType.Description,
            IsAvailable = ticketType.IsAvailable
        };
    }
}
