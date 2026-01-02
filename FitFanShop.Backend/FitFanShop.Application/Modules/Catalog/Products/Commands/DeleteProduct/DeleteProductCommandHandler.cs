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
        var variants = await _ctx.ProductVariants.Where(v => v.ProductId == product.Id).ToListAsync(ct);
        _ctx.ProductVariants.RemoveRange(variants);
        product.IsDeleted = true;
        product.IsEnabled = false;
        await _ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
