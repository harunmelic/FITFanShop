using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public DeleteProductCommand(int id) => Id = id;
}
