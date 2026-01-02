using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace FitFanShop.Infrastructure.Database.Interceptors;
public sealed class ProductCategoryValidationInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ValidateProductCategories(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ValidateProductCategories(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    private static void ValidateProductCategories(DbContext? context)
    {
        if (context == null) return;
        var productEntries = context.ChangeTracker.Entries<ProductEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();
        foreach (var entry in productEntries)
        {
            var product = entry.Entity;
            var hasCategoryInMemory = product.ProductCategories.Any();
            if (!hasCategoryInMemory)
            {
                if (entry.State == EntityState.Modified)
                {
                    var categoryCount = context.Set<ProductCategoryEntity>()
                        .Count(pc => pc.ProductId == product.Id);
                    if (categoryCount > 0)
                        continue; 
                }
                throw new FitFanShopBusinessRuleException(
                    "PRODUCT_MISSING_CATEGORY",
                    $"Proizvod '{product.Name}' mora imati barem jednu kategoriju");
            }
        }
    }
}
