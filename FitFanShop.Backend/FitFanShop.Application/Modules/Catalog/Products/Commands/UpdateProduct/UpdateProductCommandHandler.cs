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
            .FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product == null)
            throw new FitFanShopNotFoundException($"Product with id {command.Id} not found.");

        product.Name = command.Name;
        product.Description = command.Description;
        product.Price = command.Price;
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

        await _ctx.SaveChangesAsync(ct);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            IsEnabled = product.IsEnabled,
            Exclusive = product.Exclusive,
            CategoryIds = product.ProductCategories.Where(pc => !pc.IsDeleted).Select(pc => pc.CategoryId).ToList()
        };
    }
}
