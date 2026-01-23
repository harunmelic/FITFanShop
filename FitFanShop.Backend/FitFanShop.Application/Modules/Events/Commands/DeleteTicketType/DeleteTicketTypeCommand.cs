using MediatR;

namespace FitFanShop.Application.Modules.Events.Commands.DeleteTicketType;

public class DeleteTicketTypeCommand : IRequest
{
    public int EventId { get; set; }
    public int TicketTypeId { get; set; }
}
