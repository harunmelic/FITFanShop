using MediatR;
using System.Text.Json.Serialization;

namespace FitFanShop.Application.Modules.Events.Commands.AddTicketType;

public class AddTicketTypeCommand : IRequest<TicketTypeDto>
{
    [JsonIgnore]
    public int EventId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int TotalAvailable { get; set; }
    public string? Description { get; set; }
}
