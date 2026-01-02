using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class SetCategoryEnabledCommandHandler : IRequestHandler<SetCategoryEnabledCommand, Unit>
{
    private readonly IAppDbContext _ctx;
    public SetCategoryEnabledCommandHandler(IAppDbContext ctx) => _ctx = ctx;

    public async Task<Unit> Handle(SetCategoryEnabledCommand command, CancellationToken ct)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == command.Id && !c.IsDeleted, ct);
        if (category == null)
            throw new FitFanShopNotFoundException($"Category with id {command.Id} not found.");
        category.IsEnabled = command.IsEnabled;
        await _ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
