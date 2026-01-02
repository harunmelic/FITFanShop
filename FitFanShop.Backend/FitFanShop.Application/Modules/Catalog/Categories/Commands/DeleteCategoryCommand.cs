using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public DeleteCategoryCommand(int id) => Id = id;
}
