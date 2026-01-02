using FitFanShop.Domain.Entities.Identity;
using FitFanShop.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
namespace FitFanShop.Infrastructure.Database.Seeders;
public static class StaticDataSeeder
{
    private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedRoles(modelBuilder);
        SeedOrderStatuses(modelBuilder);
    }
    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleEntity>().HasData(
            new RoleEntity
            {
                Id = 1,
                Type = RoleType.User,
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            },
            new RoleEntity
            {
                Id = 2,
                Type = RoleType.Admin,
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            }
        );
    }
    private static void SeedOrderStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderStatusEntity>().HasData(
            new OrderStatusEntity
            {
                Id = 1,
                Name = "Pending",
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            },
            new OrderStatusEntity
            {
                Id = 2,
                Name = "Confirmed",
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            },
            new OrderStatusEntity
            {
                Id = 3,
                Name = "Delivered",
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            },
            new OrderStatusEntity
            {
                Id = 4,
                Name = "Cancelled",
                CreatedAtUtc = SeedDate,
                ModifiedAtUtc = null,
                IsDeleted = false
            }
        );
    }
}