using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Catalog;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IAppDbContext _ctx;

    public CreateProductCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken ct)
    {
        if (command.CategoryIds != null && command.CategoryIds.Any())
        {
            var distinctCategoryIds = command.CategoryIds.Distinct().ToList();
            var categories = await _ctx.Categories
                .Where(c => distinctCategoryIds.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync(ct);

            if (categories.Count != distinctCategoryIds.Count)
                throw new FitFanShopBusinessRuleException("CategoryNotFound", "One or more categories do not exist.");
        }

        var product = new ProductEntity
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsEnabled = command.IsEnabled,
            Exclusive = command.Exclusive,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Add ProductCategories BEFORE saving (to pass Interceptor validation)
        if (command.CategoryIds != null && command.CategoryIds.Any())
        {
            foreach (var catId in command.CategoryIds.Distinct())
            {
                product.ProductCategories.Add(new ProductCategoryEntity
                {
                    CategoryId = catId,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }
        }

        _ctx.Products.Add(product);
        
        // Add ProductVariants
        if (command.Variants != null && command.Variants.Any())
        {
            foreach (var v in command.Variants)
            {
                var variant = new ProductVariantEntity
                {
                    Product = product,
                    Size = v.Size,
                    StockQuantity = v.StockQuantity,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _ctx.ProductVariants.Add(variant);
            }
        }

        await _ctx.SaveChangesAsync(ct);

        // Generate SKU after product is saved (we need product.Id)
        if (command.Variants != null && command.Variants.Any())
        {
            var variants = await _ctx.ProductVariants
                .Where(v => v.ProductId == product.Id)
                .ToListAsync(ct);

            foreach (var variant in variants)
            {
                variant.Sku = $"FCFIT-{product.Id:D3}-{variant.Size.ToUpper()}";
            }

            await _ctx.SaveChangesAsync(ct);
        }

        var createdProduct = await _ctx.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == product.Id, ct);

        return new ProductDto
        {
            Id = createdProduct!.Id,
            Name = createdProduct.Name,
            Description = createdProduct.Description,
            Price = createdProduct.Price,
            IsEnabled = createdProduct.IsEnabled,
            Exclusive = createdProduct.Exclusive,
            CategoryIds = createdProduct.ProductCategories
                .Where(pc => !pc.IsDeleted)
                .Select(pc => pc.CategoryId)
                .ToList(),
            Variants = createdProduct.Variants
                .Where(v => !v.IsDeleted)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Size = v.Size,
                    StockQuantity = v.StockQuantity,
                    Sku = v.Sku ?? string.Empty
                })
                .ToList()
        };
    }
}
