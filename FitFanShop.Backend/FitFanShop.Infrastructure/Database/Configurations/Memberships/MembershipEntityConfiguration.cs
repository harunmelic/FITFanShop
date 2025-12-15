using FitFanShop.Domain.Entities.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Memberships;

public sealed class MembershipEntityConfiguration : IEntityTypeConfiguration<MembershipEntity>
{
    public void Configure(EntityTypeBuilder<MembershipEntity> builder)
    {
        builder.ToTable("Memberships");

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Member)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
