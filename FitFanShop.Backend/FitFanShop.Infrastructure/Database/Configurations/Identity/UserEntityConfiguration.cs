using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Memberships;
using FitFanShop.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitFanShop.Infrastructure.Database.Configurations.Identity;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<FitFanShopUserEntity>
{
    public void Configure(EntityTypeBuilder<FitFanShopUserEntity> b)
    {
        b.ToTable("Users");

        b.HasKey(x => x.Id);

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PasswordHash)
            .IsRequired();

        b.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.RoleId)
            .HasDefaultValue((int)RoleType.User);

        b.Property(x => x.TokenVersion)
            .HasDefaultValue(0);

        b.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        b.Property(x => x.SecurityQuestion)
            .HasMaxLength(200)
            .IsRequired(false);

        b.Property(x => x.SecurityAnswerHash)
            .HasMaxLength(500)
            .IsRequired(false);

        b.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany<RefreshTokenEntity>()
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        b.HasOne(x => x.MemberProfile)
            .WithOne(x => x.User)
            .HasForeignKey<MemberEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Cart)
            .WithOne(x => x.User)
            .HasForeignKey<CartEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Wishlist)
            .WithOne(x => x.User)
            .HasForeignKey<WishlistEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
