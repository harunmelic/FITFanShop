using FitFanShop.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Catalog;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("Categories");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CategoryEntity.Constraints.NameMaxLength);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        builder.HasMany(x => x.ProductCategories)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
