using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Catalog;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IAppDbContext _ctx;
    public UpdateCategoryCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == command.Id && !c.IsDeleted, ct);
        if (category == null)
            throw new FitFanShopNotFoundException($"Category with id {command.Id} not found.");
        category.Name = command.Name;
        category.Description = command.Description;
        category.IsEnabled = command.IsEnabled;
        category.ModifiedAtUtc = DateTime.UtcNow;
        await _ctx.SaveChangesAsync(ct);
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsEnabled = category.IsEnabled
        };
    }
}
