using FitFanShop.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Sales;

public sealed class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("Payments");

        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Order)
            .WithOne(o => o!.Payment)
            .HasForeignKey<PaymentEntity>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
