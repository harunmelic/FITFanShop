using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Tickets;

public sealed class EventEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public decimal TicketPrice { get; set; }

    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
}
