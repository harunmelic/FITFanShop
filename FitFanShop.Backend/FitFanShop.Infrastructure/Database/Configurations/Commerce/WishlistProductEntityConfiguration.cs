using FitFanShop.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Commerce;

public sealed class WishlistProductEntityConfiguration : IEntityTypeConfiguration<WishlistProductEntity>
{
    public void Configure(EntityTypeBuilder<WishlistProductEntity> builder)
    {
        builder.ToTable("WishlistProducts");

        builder.HasIndex(x => new { x.WishlistId, x.ProductId })
            .IsUnique();

        builder.HasOne(x => x.Wishlist)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.WishlistProducts)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
