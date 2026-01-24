using MediatR;
using System.Text.Json.Serialization;

namespace FitFanShop.Application.Modules.Reviews.Commands.UpdateReview;

public class UpdateReviewCommand : IRequest<ReviewDto>
{
    [JsonIgnore]
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
