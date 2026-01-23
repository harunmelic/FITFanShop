using FitFanShop.Application.Abstractions;
using FitFanShop.Domain.Entities.Tickets;
using MediatR;

namespace FitFanShop.Application.Modules.Events.Commands.CreateEvent;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDetailsDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IMediator _mediator;

    public CreateEventCommandHandler(IAppDbContext ctx, IMediator mediator)
    {
        _ctx = ctx;
        _mediator = mediator;
    }

    public async Task<EventDetailsDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = new EventEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            EventDate = request.EventDate,
            Location = request.Location.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _ctx.Events.Add(eventEntity);
        await _ctx.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new Queries.GetEventById.GetEventByIdQuery { Id = eventEntity.Id }, cancellationToken);
    }
}
