using FitFanShop.Domain.Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Tickets;

public sealed class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("Events");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(EventEntity.Constraints.NameMaxLength);

        builder.Property(x => x.Description)
            .HasMaxLength(EventEntity.Constraints.DescriptionMaxLength);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(EventEntity.Constraints.LocationMaxLength);

        builder.HasMany(x => x.TicketTypes)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tickets)
            .WithOne(x => x.Event)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
