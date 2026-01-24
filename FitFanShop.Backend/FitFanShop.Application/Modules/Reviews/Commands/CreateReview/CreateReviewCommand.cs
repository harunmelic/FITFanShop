using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Commands.CreateReview;

public class CreateReviewCommand : IRequest<ReviewDto>
{
    public int OrderItemId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
