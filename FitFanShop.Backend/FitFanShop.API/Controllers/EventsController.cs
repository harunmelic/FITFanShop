using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Events;
using FitFanShop.Application.Modules.Events.Commands.AddTicketType;
using FitFanShop.Application.Modules.Events.Commands.CreateEvent;
using FitFanShop.Application.Modules.Events.Commands.DeleteEvent;
using FitFanShop.Application.Modules.Events.Commands.DeleteTicketType;
using FitFanShop.Application.Modules.Events.Commands.UpdateEvent;
using FitFanShop.Application.Modules.Events.Commands.UpdateTicketType;
using FitFanShop.Application.Modules.Events.Queries.GetAllEvents;
using FitFanShop.Application.Modules.Events.Queries.GetEventById;
using FitFanShop.Application.Modules.Events.Queries.GetUpcomingEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventDetailsDto>> CreateEvent([FromBody] CreateEventCommand command)
    {
        var eventDto = await _mediator.Send(command);
        return Ok(eventDto);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PageResult<EventDto>>> GetAllEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? upcomingOnly = null)
    {
        var query = new GetAllEventsQuery
        {
            Paging = new PageRequest { Page = page, PageSize = pageSize },
            UpcomingOnly = upcomingOnly
        };
        var events = await _mediator.Send(query);
        return Ok(events);
    }

    [HttpGet("upcoming")]
    [AllowAnonymous]
    public async Task<ActionResult<List<EventDto>>> GetUpcomingEvents()
    {
        var events = await _mediator.Send(new GetUpcomingEventsQuery());
        return Ok(events);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDetailsDto>> GetEventById(int id)
    {
        var eventDto = await _mediator.Send(new GetEventByIdQuery { Id = id });
        return Ok(eventDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventDetailsDto>> UpdateEvent(int id, [FromBody] UpdateEventCommand command)
    {
        command.Id = id;
        var eventDto = await _mediator.Send(command);
        return Ok(eventDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        await _mediator.Send(new DeleteEventCommand { Id = id });
        return NoContent();
    }

    [HttpPost("{eventId}/ticket-types")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketTypeDto>> AddTicketType(int eventId, [FromBody] AddTicketTypeCommand command)
    {
        command.EventId = eventId;
        var ticketType = await _mediator.Send(command);
        return Ok(ticketType);
    }

    [HttpPut("{eventId}/ticket-types/{ticketTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TicketTypeDto>> UpdateTicketType(
        int eventId, 
        int ticketTypeId, 
        [FromBody] UpdateTicketTypeCommand command)
    {
        command.EventId = eventId;
        command.TicketTypeId = ticketTypeId;
        var ticketType = await _mediator.Send(command);
        return Ok(ticketType);
    }

    [HttpDelete("{eventId}/ticket-types/{ticketTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTicketType(int eventId, int ticketTypeId)
    {
        await _mediator.Send(new DeleteTicketTypeCommand { EventId = eventId, TicketTypeId = ticketTypeId });
        return NoContent();
    }
}
