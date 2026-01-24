using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Queries.GetReviewById;

public class GetReviewByIdQuery : IRequest<ReviewDto>
{
    public int Id { get; set; }

    public GetReviewByIdQuery(int id)
    {
        Id = id;
    }
}
