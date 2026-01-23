using FitFanShop.Application.Common;
using MediatR;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetAllDiscounts;

public class GetAllDiscountsQuery : IRequest<PageResult<DiscountDto>>
{
    public PageRequest Paging { get; set; } = new();
    public bool? ActiveOnly { get; set; }
    public bool? MembersOnly { get; set; }
}
