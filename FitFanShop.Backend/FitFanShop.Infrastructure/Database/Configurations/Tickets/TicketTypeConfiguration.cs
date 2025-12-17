using FitFanShop.Domain.Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Tickets;

public sealed class TicketTypeConfiguration : IEntityTypeConfiguration<TicketTypeEntity>
{
    public void Configure(EntityTypeBuilder<TicketTypeEntity> builder)
    {
        builder.ToTable("TicketTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(TicketTypeEntity.Constraints.NameMaxLength);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAvailable)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(TicketTypeEntity.Constraints.DescriptionMaxLength);

        // Relationship: TicketType -> Event (Many-to-One)
        builder.HasOne(x => x.Event)
            .WithMany(x => x.TicketTypes)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for faster lookups
        builder.HasIndex(x => x.EventId);
    }
}
