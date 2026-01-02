using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IAppDbContext _ctx;
    public DeleteCategoryCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == command.Id && !c.IsDeleted, ct);
        if (category == null)
            throw new FitFanShopNotFoundException($"Category with id {command.Id} not found.");
        category.IsDeleted = true;
        category.IsEnabled = false;
        await _ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
