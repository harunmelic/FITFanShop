using FitFanShop.Domain.Entities.Discounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Discounts;

public sealed class DiscountProductEntityConfiguration : IEntityTypeConfiguration<DiscountProductEntity>
{
    public void Configure(EntityTypeBuilder<DiscountProductEntity> builder)
    {
        builder.ToTable("DiscountProducts");

        builder.HasIndex(x => new { x.DiscountId, x.ProductId })
            .IsUnique();

        builder.HasOne(x => x.Discount)
            .WithMany(x => x.DiscountProducts)
            .HasForeignKey(x => x.DiscountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.DiscountProducts)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
