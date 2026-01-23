using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Commands.DeleteEvent;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
{
    private readonly IAppDbContext _ctx;

    public DeleteEventCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _ctx.Events
            .Include(e => e.TicketTypes)
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (eventEntity == null)
            throw new FitFanShopNotFoundException($"Event with id {request.Id} not found.");

        eventEntity.IsDeleted = true;
        eventEntity.ModifiedAtUtc = DateTime.UtcNow;

        foreach (var ticketType in eventEntity.TicketTypes.Where(tt => !tt.IsDeleted))
        {
            ticketType.IsDeleted = true;
            ticketType.ModifiedAtUtc = DateTime.UtcNow;
        }

        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
