using FitFanShop.Domain.Entities.Discounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Discounts;

public sealed class DiscountEntityConfiguration : IEntityTypeConfiguration<DiscountEntity>
{
    public void Configure(EntityTypeBuilder<DiscountEntity> builder)
    {
        builder.ToTable("Discounts");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Percentage)
            .HasPrecision(5, 2);

        builder.HasMany(x => x.DiscountProducts)
            .WithOne(x => x.Discount)
            .HasForeignKey(x => x.DiscountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
