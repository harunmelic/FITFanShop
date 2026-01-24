using MediatR;

namespace FitFanShop.Application.Modules.Reviews.Commands.DeleteReview;

public class DeleteReviewCommand : IRequest<Unit>
{
    public int Id { get; set; }

    public DeleteReviewCommand(int id)
    {
        Id = id;
    }
}
