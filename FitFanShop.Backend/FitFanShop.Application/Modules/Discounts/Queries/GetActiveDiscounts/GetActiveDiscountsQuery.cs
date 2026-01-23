using MediatR;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetActiveDiscounts;

public class GetActiveDiscountsQuery : IRequest<List<DiscountDto>>
{
}
