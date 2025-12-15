using FitFanShop.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Identity;

public sealed class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.Type)
            .IsUnique();
    }
}
