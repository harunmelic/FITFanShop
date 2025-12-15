using FitFanShop.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Commerce;

public sealed class WishlistEntityConfiguration : IEntityTypeConfiguration<WishlistEntity>
{
    public void Configure(EntityTypeBuilder<WishlistEntity> builder)
    {
        builder.ToTable("Wishlists");

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Wishlist)
            .HasForeignKey(x => x.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
