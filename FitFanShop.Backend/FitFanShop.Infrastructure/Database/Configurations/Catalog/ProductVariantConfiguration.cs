using FitFanShop.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Catalog;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariantEntity>
{
    public void Configure(EntityTypeBuilder<ProductVariantEntity> builder)
    {
        builder.ToTable("ProductVariants");

        builder.Property(x => x.Size)
            .IsRequired()
            .HasMaxLength(ProductVariantEntity.Constraints.SizeMaxLength);

        builder.Property(x => x.Sku)
            .HasMaxLength(ProductVariantEntity.Constraints.SkuMaxLength);

        builder.Property(x => x.StockQuantity)
            .IsRequired();

        // Relationship: ProductVariant -> Product (Many-to-One)
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for faster lookups
        builder.HasIndex(x => new { x.ProductId, x.Size });
        builder.HasIndex(x => x.Sku);
    }
}
