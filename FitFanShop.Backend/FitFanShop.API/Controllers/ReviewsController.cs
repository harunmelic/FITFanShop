using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Reviews;
using FitFanShop.Application.Modules.Reviews.Commands.CreateReview;
using FitFanShop.Application.Modules.Reviews.Commands.DeleteReview;
using FitFanShop.Application.Modules.Reviews.Commands.UpdateReview;
using FitFanShop.Application.Modules.Reviews.Queries.GetMyReviews;
using FitFanShop.Application.Modules.Reviews.Queries.GetProductReviews;
using FitFanShop.Application.Modules.Reviews.Queries.GetProductReviewStats;
using FitFanShop.Application.Modules.Reviews.Queries.GetReviewById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new review for a purchased product
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetReviewById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get all reviews for a specific product (paginated)
    /// </summary>
    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PageResult<ReviewDto>>> GetProductReviews(
        int productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? rating = null,
        [FromQuery] string? sortBy = "date_desc")
    {
        var query = new GetProductReviewsQuery
        {
            ProductId = productId,
            Rating = rating,
            SortBy = sortBy,
            Paging = new PageRequest { Page = page, PageSize = pageSize }
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get review statistics for a specific product
    /// </summary>
    [HttpGet("product/{productId}/stats")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductReviewStatsDto>> GetProductReviewStats(int productId)
    {
        var result = await _mediator.Send(new GetProductReviewStatsQuery(productId));
        return Ok(result);
    }

    /// <summary>
    /// Get all reviews created by the current user (paginated)
    /// </summary>
    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<ActionResult<PageResult<ReviewDto>>> GetMyReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? productId = null,
        [FromQuery] string? sortBy = "date_desc")
    {
        var query = new GetMyReviewsQuery
        {
            ProductId = productId,
            SortBy = sortBy,
            Paging = new PageRequest { Page = page, PageSize = pageSize }
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific review by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
    {
        var result = await _mediator.Send(new GetReviewByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Update an existing review (only by owner)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> UpdateReview(int id, [FromBody] UpdateReviewCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a review (owner or admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _mediator.Send(new DeleteReviewCommand(id));
        return NoContent();
    }
}
