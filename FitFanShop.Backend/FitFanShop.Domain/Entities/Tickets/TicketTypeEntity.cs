using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Commerce;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitFanShop.Domain.Entities.Tickets;

public sealed class TicketTypeEntity : BaseEntity
{
    public int EventId { get; set; }
    public EventEntity? Event { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalAvailable { get; set; }
    public string? Description { get; set; }
    [NotMapped]
    public bool IsAvailable => TotalAvailable > 0 && Event?.TicketsAvailable == true;
    public ICollection<TicketEntity> Tickets { get; private set; } = new List<TicketEntity>();
    public ICollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();
    public static class Constraints
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;
    }
}
