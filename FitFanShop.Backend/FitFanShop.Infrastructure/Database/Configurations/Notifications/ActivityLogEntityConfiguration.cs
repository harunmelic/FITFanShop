using FitFanShop.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Notifications;

public sealed class ActivityLogEntityConfiguration : IEntityTypeConfiguration<ActivityLogEntity>
{
    public void Configure(EntityTypeBuilder<ActivityLogEntity> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.Property(x => x.ActionDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ActivityLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
