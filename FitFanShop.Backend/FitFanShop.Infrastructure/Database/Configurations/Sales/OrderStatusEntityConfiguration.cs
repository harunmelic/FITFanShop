using FitFanShop.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Sales;

public sealed class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatusEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusEntity> builder)
    {
        builder.ToTable("OrderStatuses");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.Status)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
