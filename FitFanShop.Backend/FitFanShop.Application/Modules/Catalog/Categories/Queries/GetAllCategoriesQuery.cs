using MediatR;
using System.Collections.Generic;

namespace FitFanShop.Application.Modules.Catalog.Categories.Queries;

public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
