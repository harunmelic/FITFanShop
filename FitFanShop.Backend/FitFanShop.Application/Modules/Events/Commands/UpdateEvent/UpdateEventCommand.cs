using MediatR;
using System.Text.Json.Serialization;

namespace FitFanShop.Application.Modules.Events.Commands.UpdateEvent;

public class UpdateEventCommand : IRequest<EventDetailsDto>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public required string Location { get; set; }
}
