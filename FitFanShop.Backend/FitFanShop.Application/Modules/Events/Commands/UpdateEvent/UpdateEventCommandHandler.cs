using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Events.Commands.UpdateEvent;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, EventDetailsDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IMediator _mediator;

    public UpdateEventCommandHandler(IAppDbContext ctx, IMediator mediator)
    {
        _ctx = ctx;
        _mediator = mediator;
    }

    public async Task<EventDetailsDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _ctx.Events
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (eventEntity == null)
            throw new FitFanShopNotFoundException($"Event with id {request.Id} not found.");

        eventEntity.Name = request.Name.Trim();
        eventEntity.Description = request.Description?.Trim();
        eventEntity.EventDate = request.EventDate;
        eventEntity.Location = request.Location.Trim();
        eventEntity.ModifiedAtUtc = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetEventById.GetEventByIdQuery { Id = eventEntity.Id }, cancellationToken);
    }
}
