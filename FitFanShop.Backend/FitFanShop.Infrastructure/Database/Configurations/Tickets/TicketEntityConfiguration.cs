using FitFanShop.Domain.Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Tickets;

public sealed class TicketEntityConfiguration : IEntityTypeConfiguration<TicketEntity>
{
    public void Configure(EntityTypeBuilder<TicketEntity> builder)
    {
        builder.ToTable("Tickets");

        builder.Property(x => x.PricePaid)
            .HasPrecision(18, 2);

        builder.Property(x => x.QRCode)
            .IsRequired()
            .HasMaxLength(TicketEntity.Constraints.QRCodeMaxLength);

        builder.Property(x => x.SeatNumber)
            .HasMaxLength(TicketEntity.Constraints.SeatNumberMaxLength);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(TicketEntity.Constraints.StatusMaxLength);

        builder.HasOne(x => x.TicketType)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for QR code lookups (check-in)
        builder.HasIndex(x => x.QRCode)
            .IsUnique();

        // Index for user tickets
        builder.HasIndex(x => x.UserId);
    }
}
