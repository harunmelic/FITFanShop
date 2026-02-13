using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Catalog;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IAppDbContext _ctx;
    public UpdateProductCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await _ctx.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.Variants)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product == null)
            throw new FitFanShopNotFoundException($"Product with id {command.Id} not found.");

        product.Name = command.Name;
        product.Description = command.Description;
        product.Price = command.Price;
        product.Image = command.ImageUrl;
        product.IsEnabled = command.IsEnabled;
        product.Exclusive = command.Exclusive;

        if (command.CategoryIds != null)
        {
            var distinctCategoryIds = command.CategoryIds.Distinct().ToList();
            var categories = await _ctx.Categories
                .Where(c => distinctCategoryIds.Contains(c.Id))
                .ToListAsync(ct);

            if (categories.Count != distinctCategoryIds.Count)
                throw new FitFanShopBusinessRuleException("CategoryNotFound", "One or more categories do not exist.");

            var allProductCategories = await _ctx.ProductCategories
                .Where(pc => pc.ProductId == product.Id)
                .IgnoreQueryFilters()
                .ToListAsync(ct);

            var existingCategoryIds = allProductCategories
                .Where(pc => !pc.IsDeleted)
                .Select(pc => pc.CategoryId)
                .ToList();
            var toAdd = distinctCategoryIds.Except(existingCategoryIds).ToList();
            var toRemove = existingCategoryIds.Except(distinctCategoryIds).ToList();

            foreach (var catId in toAdd)
            {
                var existing = allProductCategories.FirstOrDefault(pc => pc.CategoryId == catId);
                if (existing != null)
                {
                    if (existing.IsDeleted)
                        existing.IsDeleted = false;
                }
                else
                {
                    product.ProductCategories.Add(new ProductCategoryEntity { CategoryId = catId, ProductId = product.Id });
                }
            }

            foreach (var catId in toRemove)
            {
                var toDelete = allProductCategories.FirstOrDefault(pc => pc.CategoryId == catId);
                if (toDelete != null)
                {
                    toDelete.IsDeleted = true;
                }
            }
        }

        if (command.Variants != null)
        {
            var incomingVariantIds = command.Variants
                .Where(v => v.Id.HasValue)
                .Select(v => v.Id!.Value)
                .ToHashSet();

            var existingVariants = product.Variants.Where(v => !v.IsDeleted).ToList();

            // Remove variants not in the incoming list
            foreach (var existing in existingVariants)
            {
                if (!incomingVariantIds.Contains(existing.Id))
                {
                    existing.IsDeleted = true;
                }
            }

            foreach (var v in command.Variants)
            {
                if (v.Id.HasValue)
                {
                    // Update existing variant
                    var existing = product.Variants.FirstOrDefault(e => e.Id == v.Id.Value && !e.IsDeleted);
                    if (existing != null)
                    {
                        existing.Size = v.Size;
                        existing.StockQuantity = v.StockQuantity;
                    }
                }
                else
                {
                    // Create new variant
                    var newVariant = new ProductVariantEntity
                    {
                        ProductId = product.Id,
                        Size = v.Size,
                        StockQuantity = v.StockQuantity,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    product.Variants.Add(newVariant);
                }
            }
        }

        await _ctx.SaveChangesAsync(ct);

        // Generate SKU for new variants that don't have one
        var variantsWithoutSku = product.Variants.Where(v => !v.IsDeleted && string.IsNullOrEmpty(v.Sku)).ToList();
        if (variantsWithoutSku.Any())
        {
            foreach (var variant in variantsWithoutSku)
            {
                variant.Sku = $"FCFIT-{product.Id:D3}-{variant.Size.ToUpper()}";
            }
            await _ctx.SaveChangesAsync(ct);
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.Image,
            IsEnabled = product.IsEnabled,
            Exclusive = product.Exclusive,
            CategoryIds = product.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => pc.CategoryId).ToList(),
            Variants = product.Variants
                .Where(v => !v.IsDeleted)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Size = v.Size,
                    StockQuantity = v.StockQuantity,
                    Sku = v.Sku ?? string.Empty
                })
                .ToList(),
            ReviewCount = product.Reviews.Count(r => !r.IsDeleted),
            AverageRating = product.Reviews.Any(r => !r.IsDeleted) 
                ? (decimal)Math.Round(product.Reviews.Where(r => !r.IsDeleted).Average(r => r.Rating), 2) 
                : 0
        };
    }
}
