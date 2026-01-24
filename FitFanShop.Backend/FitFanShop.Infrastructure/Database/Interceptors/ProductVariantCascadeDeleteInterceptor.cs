using FitFanShop.Domain.Common;
using FitFanShop.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FitFanShop.Infrastructure.Database.Interceptors;

public class ProductVariantCascadeDeleteInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var context = eventData.Context;

        // Pronaði sve Product entitete koji se soft-deletuju
        var deletedProducts = context.ChangeTracker
            .Entries<ProductEntity>()
            .Where(e => e.State == EntityState.Modified && 
                       e.Entity.IsDeleted && 
                       e.OriginalValues.GetValue<bool>(nameof(BaseEntity.IsDeleted)) == false)
            .Select(e => e.Entity.Id)
            .ToList();

        if (deletedProducts.Any())
        {
            // Soft-deletuj sve ProductVariants koji pripadaju obrisanim Products
            var variantsToDelete = await context.Set<ProductVariantEntity>()
                .Where(v => deletedProducts.Contains(v.ProductId) && !v.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var variant in variantsToDelete)
            {
                variant.IsDeleted = true;
                variant.ModifiedAtUtc = DateTime.UtcNow;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
