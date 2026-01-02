using FitFanShop.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace FitFanShop.Infrastructure.Database.Configurations.Commerce;
public sealed class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        builder.ToTable("CartItems");
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_CartItem_OneType",
            "(ProductVariantId IS NOT NULL AND TicketTypeId IS NULL) OR (ProductVariantId IS NULL AND TicketTypeId IS NOT NULL)"
        ));
        builder.HasOne(x => x.Cart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TicketType)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
