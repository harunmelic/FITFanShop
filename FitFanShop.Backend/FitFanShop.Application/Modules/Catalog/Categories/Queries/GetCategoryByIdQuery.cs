using MediatR;

namespace FitFanShop.Application.Modules.Catalog.Categories.Queries;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public int Id { get; set; }
    public GetCategoryByIdQuery(int id) => Id = id;
}
