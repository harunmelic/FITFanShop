using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Commands.DeleteTicketType;

public class DeleteTicketTypeCommandHandler : IRequestHandler<DeleteTicketTypeCommand>
{
    private readonly IAppDbContext _ctx;

    public DeleteTicketTypeCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ctx.TicketTypes
            .FirstOrDefaultAsync(tt => 
                tt.Id == request.TicketTypeId && 
                tt.EventId == request.EventId && 
                !tt.IsDeleted, 
                cancellationToken);

        if (ticketType == null)
            throw new FitFanShopNotFoundException($"Ticket type with id {request.TicketTypeId} not found for event {request.EventId}.");

        ticketType.IsDeleted = true;
        ticketType.ModifiedAtUtc = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
