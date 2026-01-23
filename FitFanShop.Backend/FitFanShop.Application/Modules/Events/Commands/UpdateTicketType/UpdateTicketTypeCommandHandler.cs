using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Commands.UpdateTicketType;

public class UpdateTicketTypeCommandHandler : IRequestHandler<UpdateTicketTypeCommand, TicketTypeDto>
{
    private readonly IAppDbContext _ctx;

    public UpdateTicketTypeCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<TicketTypeDto> Handle(UpdateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ctx.TicketTypes
            .Include(tt => tt.Event)
            .FirstOrDefaultAsync(tt => 
                tt.Id == request.TicketTypeId && 
                tt.EventId == request.EventId && 
                !tt.IsDeleted, 
                cancellationToken);

        if (ticketType == null)
            throw new FitFanShopNotFoundException($"Ticket type with id {request.TicketTypeId} not found for event {request.EventId}.");

        ticketType.Name = request.Name.Trim();
        ticketType.Price = request.Price;
        ticketType.TotalAvailable = request.TotalAvailable;
        ticketType.Description = request.Description?.Trim();
        ticketType.ModifiedAtUtc = DateTime.UtcNow;

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
