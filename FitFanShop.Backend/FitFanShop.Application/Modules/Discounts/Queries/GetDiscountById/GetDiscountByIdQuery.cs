using MediatR;

namespace FitFanShop.Application.Modules.Discounts.Queries.GetDiscountById;

public class GetDiscountByIdQuery : IRequest<DiscountDetailsDto>
{
    public int Id { get; set; }
}
