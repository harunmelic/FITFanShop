using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FitFanShop.Application.Modules.Catalog.Products.Commands.DeleteProduct;
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IAppDbContext _ctx;
    public DeleteProductCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await _ctx.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == command.Id, ct);
        if (product == null)
            throw new FitFanShopNotFoundException($"Product with id {command.Id} not found.");
        
        product.ProductCategories.Clear();
        
        // Soft delete Product - ProductVariantCascadeDeleteInterceptor æe automatski soft-deletovati sve variants
        product.IsDeleted = true;
        product.IsEnabled = false;
        
        await _ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
