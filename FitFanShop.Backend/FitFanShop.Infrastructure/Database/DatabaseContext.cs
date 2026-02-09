using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using FitFanShop.Application.Abstractions;
using FitFanShop.Domain.Entities.Catalog;
using FitFanShop.Domain.Entities.Commerce;
using FitFanShop.Domain.Entities.Discounts;
using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Media;
using FitFanShop.Domain.Entities.Memberships;
using FitFanShop.Domain.Entities.Notifications;
using FitFanShop.Domain.Entities.Reviews;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Domain.Entities.Tickets;
using FitFanShop.Domain.Common;

namespace FitFanShop.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductVariantEntity> ProductVariants => Set<ProductVariantEntity>();

    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<FitFanShopUserEntity> Users => Set<FitFanShopUserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DbSet<MemberEntity> Members => Set<MemberEntity>();

    public DbSet<CartEntity> Carts => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<WishlistEntity> Wishlists => Set<WishlistEntity>();
    public DbSet<WishlistProductEntity> WishlistProducts => Set<WishlistProductEntity>();

    public DbSet<DiscountEntity> Discounts => Set<DiscountEntity>();
    public DbSet<DiscountProductEntity> DiscountProducts => Set<DiscountProductEntity>();

    public DbSet<OrderStatusEntity> OrderStatuses => Set<OrderStatusEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();

    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();

    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<ActivityLogEntity> ActivityLogs => Set<ActivityLogEntity>();

    public DbSet<EventEntity> Events => Set<EventEntity>();
    public DbSet<TicketTypeEntity> TicketTypes => Set<TicketTypeEntity>();
    public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
    public DbSet<ImageEntity> Images => Set<ImageEntity>();

    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }

}