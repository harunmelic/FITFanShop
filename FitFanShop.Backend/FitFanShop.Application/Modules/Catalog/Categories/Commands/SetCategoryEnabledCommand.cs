using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class SetCategoryEnabledCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
}
