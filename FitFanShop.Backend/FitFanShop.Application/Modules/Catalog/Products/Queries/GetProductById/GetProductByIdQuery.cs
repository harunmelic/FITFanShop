using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
    public GetProductByIdQuery(int id) => Id = id;
}
