using FitFanShop.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace FitFanShop.Infrastructure.Database.Interceptors;
public sealed class StockReductionInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        await ProcessStockReductionAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    private static async Task ProcessStockReductionAsync(DbContext context, CancellationToken cancellationToken)
    {
        var modifiedOrders = context.ChangeTracker.Entries<OrderEntity>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();
        foreach (var orderEntry in modifiedOrders)
        {
            var order = orderEntry.Entity;
            var originalStatusId = orderEntry.OriginalValues.GetValue<int>(nameof(OrderEntity.StatusId));
            var currentStatusId = order.StatusId;
            if (originalStatusId != 3 && currentStatusId == 3)
            {
                await context.Entry(order)
                    .Collection(o => o.Items)
                    .Query()
                    .Include(oi => oi.ProductVariant)
                    .LoadAsync(cancellationToken);
                foreach (var orderItem in order.Items)
                {
                    if (orderItem.ProductVariant is not null)
                    {
                        orderItem.ProductVariant.StockQuantity -= orderItem.Quantity;
                        if (orderItem.ProductVariant.StockQuantity < 0)
                        {
                            throw new InvalidOperationException(
                                $"Insufficient stock for product variant (SKU: {orderItem.ProductVariant.Sku}). " +
                                $"Required: {orderItem.Quantity}, Available: {orderItem.ProductVariant.StockQuantity + orderItem.Quantity}");
                        }
                        context.Entry(orderItem.ProductVariant).State = EntityState.Modified;
                    }
                }
            }
        }
    }
}
