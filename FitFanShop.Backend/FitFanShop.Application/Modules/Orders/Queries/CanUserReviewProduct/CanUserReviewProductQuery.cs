using MediatR;

namespace FitFanShop.Application.Modules.Orders.Queries.CanUserReviewProduct;

public class CanUserReviewProductQuery : IRequest<CanUserReviewProductDto>
{
    public int ProductId { get; set; }
}
