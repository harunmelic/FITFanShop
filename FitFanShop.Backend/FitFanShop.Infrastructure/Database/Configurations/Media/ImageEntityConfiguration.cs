using FitFanShop.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Media;

public sealed class ImageEntityConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.ToTable("Images");

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(ImageEntity.Constraints.FileNameMaxLength);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(ImageEntity.Constraints.FileUrlMaxLength);

        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(ImageEntity.Constraints.MimeTypeMaxLength);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.UploadDateUtc)
            .IsRequired();

        builder.HasIndex(x => x.UploadDateUtc);
    }
}
