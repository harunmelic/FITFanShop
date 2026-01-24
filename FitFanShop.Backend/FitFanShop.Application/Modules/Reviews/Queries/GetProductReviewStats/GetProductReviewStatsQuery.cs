using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetProductReviewStats;

public class GetProductReviewStatsQuery : IRequest<ProductReviewStatsDto>
{
    public int ProductId { get; set; }

    public GetProductReviewStatsQuery(int productId)
    {
        ProductId = productId;
    }
}
