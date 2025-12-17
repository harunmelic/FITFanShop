using FitFanShop.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Tickets;

public sealed class EventEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Computed property - checks if event has already passed
    /// </summary>
    [NotMapped]
    public bool HasPassed => EventDate < DateTime.UtcNow;

    /// <summary>
    /// Computed property - checks if tickets can still be purchased
    /// </summary>
    [NotMapped]
    public bool TicketsAvailable => !HasPassed;

    public ICollection<TicketTypeEntity> TicketTypes { get; private set; } = new List<TicketTypeEntity>();
    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();

    public static class Constraints
    {
        public const int NameMaxLength = 200;
        public const int LocationMaxLength = 200;
        public const int DescriptionMaxLength = 1000;
    }
}
