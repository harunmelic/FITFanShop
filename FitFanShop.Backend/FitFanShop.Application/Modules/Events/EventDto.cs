namespace FitFanShop.Application.Modules.Events;

public class EventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool HasPassed { get; set; }
    public bool TicketsAvailable { get; set; }
    public int TotalTicketTypes { get; set; }
}
