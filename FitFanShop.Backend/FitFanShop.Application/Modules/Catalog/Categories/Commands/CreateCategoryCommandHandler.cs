using FitFanShop.Application.Abstractions;
using MediatR;
using FitFanShop.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IAppDbContext _ctx;
    public CreateCategoryCommandHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var category = new CategoryEntity
        {
            Name = command.Name,
            Description = command.Description,
            IsEnabled = command.IsEnabled,
            CreatedAtUtc = DateTime.UtcNow
        };
        _ctx.Categories.Add(category);
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
