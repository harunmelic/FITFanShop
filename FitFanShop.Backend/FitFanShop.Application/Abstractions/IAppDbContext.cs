using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Discounts;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Memberships;
using FitFanShop.Domain.Entities.Notifications;
using FitFanShop.Domain.Entities.Reviews;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Domain.Entities.Tickets;

namespace FitFanShop.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<CategoryEntity> Categories { get; }
    DbSet<ProductEntity> Products { get; }
    DbSet<ProductCategoryEntity> ProductCategories { get; }
    DbSet<ProductVariantEntity> ProductVariants { get; }

    DbSet<RoleEntity> Roles { get; }
    DbSet<FitFanShopUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    DbSet<MemberEntity> Members { get; }

    DbSet<CartEntity> Carts { get; }
    DbSet<CartItemEntity> CartItems { get; }
    DbSet<WishlistEntity> Wishlists { get; }
    DbSet<WishlistProductEntity> WishlistProducts { get; }

    DbSet<DiscountEntity> Discounts { get; }
    DbSet<DiscountProductEntity> DiscountProducts { get; }

    DbSet<OrderStatusEntity> OrderStatuses { get; }
    DbSet<OrderEntity> Orders { get; }
    DbSet<OrderItemEntity> OrderItems { get; }
    DbSet<PaymentEntity> Payments { get; }

    DbSet<ReviewEntity> Reviews { get; }

    DbSet<NotificationEntity> Notifications { get; }
    DbSet<ActivityLogEntity> ActivityLogs { get; }

    DbSet<EventEntity> Events { get; }
    DbSet<TicketTypeEntity> TicketTypes { get; }
    DbSet<TicketEntity> Tickets { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}