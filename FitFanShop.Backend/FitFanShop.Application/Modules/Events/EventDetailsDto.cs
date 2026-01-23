namespace FitFanShop.Application.Modules.Events;

public class EventDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool HasPassed { get; set; }
    public bool TicketsAvailable { get; set; }
    public List<TicketTypeDto> TicketTypes { get; set; } = new();
}

public class TicketTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalAvailable { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
}
