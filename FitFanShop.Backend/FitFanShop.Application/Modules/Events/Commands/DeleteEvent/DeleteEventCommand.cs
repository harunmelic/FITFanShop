using MediatR;

namespace FitFanShop.Application.Modules.Events.Commands.DeleteEvent;

public class DeleteEventCommand : IRequest
{
    public int Id { get; set; }
}
