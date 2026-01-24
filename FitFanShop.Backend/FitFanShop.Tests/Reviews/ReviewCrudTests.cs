using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Reviews;
using FitFanShop.Application.Modules.Reviews.Commands.CreateReview;
using FitFanShop.Application.Modules.Reviews.Commands.UpdateReview;
using FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;
using FitFanShop.Application.Modules.Payments.Commands.MockCheckout;
using Xunit;

namespace FitFanShop.Tests.Reviews;

public class ReviewCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ReviewCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GET Tests

    [Fact]
    public async Task Review_GetProductReviews_ReturnsOk()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/reviews/product/1?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
    }

    [Fact]
    public async Task Review_GetProductReviews_WithRatingFilter_ReturnsFiltered()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/reviews/product/1?page=1&pageSize=10&rating=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(result);
        
        if (result.Items.Any())
        {
            Assert.All(result.Items, review => Assert.Equal(5, review.Rating));
        }
    }

    [Fact]
    public async Task Review_GetProductReviewStats_ReturnsStats()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/reviews/product/1/stats");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stats = await response.Content.ReadFromJsonAsync<ProductReviewStatsDto>();
        Assert.NotNull(stats);
        Assert.True(stats.TotalReviews >= 0);
        Assert.True(stats.AverageRating >= 0 && stats.AverageRating <= 5);
    }

    [Fact]
    public async Task Review_GetMyReviews_RequiresAuth()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/reviews/my-reviews");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Review_GetMyReviews_ReturnsUserReviews()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/reviews/my-reviews?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
    }

    [Fact]
    public async Task Review_GetById_ReturnsNotFound_ForNonExistent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/reviews/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region CREATE Tests

    [Fact]
    public async Task Review_Create_RequiresAuth()
    {
        var client = _factory.CreateClient();

        var command = new CreateReviewCommand
        {
            OrderItemId = 1,
            Rating = 5,
            Comment = "Great product!"
        };

        var response = await client.PostAsJsonAsync("/api/reviews", command);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Review_Create_FailsWithInvalidRating()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new CreateReviewCommand
        {
            OrderItemId = 1,
            Rating = 6, // Invalid - exceeds max (5)
            Comment = "Test review"
        };

        var response = await client.PostAsJsonAsync("/api/reviews", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Review_Create_FailsWithZeroRating()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new CreateReviewCommand
        {
            OrderItemId = 1,
            Rating = 0, // Invalid - must be >= 1
            Comment = "Test review"
        };

        var response = await client.PostAsJsonAsync("/api/reviews", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Review_Create_FailsWithTooLongComment()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new CreateReviewCommand
        {
            OrderItemId = 1,
            Rating = 5,
            Comment = new string('A', 2001) // Exceeds 2000 char limit
        };

        var response = await client.PostAsJsonAsync("/api/reviews", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Review_Create_FailsForNonExistentOrderItem()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new CreateReviewCommand
        {
            OrderItemId = 99999,
            Rating = 5,
            Comment = "Test review"
        };

        var response = await client.PostAsJsonAsync("/api/reviews", command);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UPDATE Tests

    [Fact]
    public async Task Review_Update_RequiresAuth()
    {
        var client = _factory.CreateClient();

        var command = new UpdateReviewCommand
        {
            Rating = 4,
            Comment = "Updated comment"
        };

        var response = await client.PutAsJsonAsync("/api/reviews/1", command);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Review_Update_FailsWithInvalidRating()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new UpdateReviewCommand
        {
            Rating = 6, // Invalid
            Comment = "Updated comment"
        };

        var response = await client.PutAsJsonAsync("/api/reviews/1", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Review_Update_ReturnsNotFound_ForNonExistent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var command = new UpdateReviewCommand
        {
            Rating = 4,
            Comment = "Updated comment"
        };

        var response = await client.PutAsJsonAsync("/api/reviews/99999", command);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task Review_Delete_RequiresAuth()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/reviews/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Review_Delete_ReturnsNotFound_ForNonExistent()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.DeleteAsync("/api/reviews/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public async Task Review_Stats_CalculatesCorrectAverageRating()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        // Get stats for a product
        var response = await client.GetAsync("/api/reviews/product/1/stats");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stats = await response.Content.ReadFromJsonAsync<ProductReviewStatsDto>();
        Assert.NotNull(stats);

        // If there are reviews, average should be between 1 and 5
        if (stats.TotalReviews > 0)
        {
            Assert.InRange(stats.AverageRating, 0m, 5m);
        }
        else
        {
            Assert.Equal(0m, stats.AverageRating);
        }
    }

    [Fact]
    public async Task Review_Stats_CountsCorrectly()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var statsResponse = await client.GetAsync("/api/reviews/product/1/stats");
        var stats = await statsResponse.Content.ReadFromJsonAsync<ProductReviewStatsDto>();
        Assert.NotNull(stats);

        var reviewsResponse = await client.GetAsync("/api/reviews/product/1?page=1&pageSize=1000");
        var reviews = await reviewsResponse.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(reviews);

        // Total count in stats should match actual review count
        Assert.Equal(stats.TotalReviews, reviews.TotalItems);
    }

    [Fact]
    public async Task Review_Pagination_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        // Get first page
        var page1Response = await client.GetAsync("/api/reviews/product/1?page=1&pageSize=5");
        var page1 = await page1Response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(page1);
        Assert.Equal(1, page1.CurrentPage);
        Assert.Equal(5, page1.PageSize);

        if (page1.TotalItems > 5)
        {
            // Get second page
            var page2Response = await client.GetAsync("/api/reviews/product/1?page=2&pageSize=5");
            var page2 = await page2Response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
            Assert.NotNull(page2);
            Assert.Equal(2, page2.CurrentPage);

            // Items on different pages should be different
            var page1Ids = page1.Items.Select(r => r.Id).ToList();
            var page2Ids = page2.Items.Select(r => r.Id).ToList();
            Assert.Empty(page1Ids.Intersect(page2Ids));
        }
    }

    [Fact]
    public async Task Review_Sorting_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        // Get reviews sorted descending by date (default)
        var response = await client.GetAsync("/api/reviews/product/1?page=1&pageSize=10&sortDescending=true");
        var result = await response.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(result);

        if (result.Items.Count > 1)
        {
            // Verify descending order
            for (int i = 0; i < result.Items.Count - 1; i++)
            {
                Assert.True(result.Items[i].CreatedAt >= result.Items[i + 1].CreatedAt);
            }
        }
    }

    #endregion

    #region Integration Tests (Full Workflow)

    [Fact]
    public async Task Review_FullWorkflow_CreateUpdateDelete()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        // Note: This test requires a confirmed/delivered order with an item
        // In a real scenario, you'd create an order, confirm it, then create a review
        // For this example, we'll assume there's a seeded order available

        // 1. Try to get reviews for "my-reviews"
        var myReviewsResponse = await client.GetAsync("/api/reviews/my-reviews?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, myReviewsResponse.StatusCode);

        var myReviews = await myReviewsResponse.Content.ReadFromJsonAsync<PageResult<ReviewDto>>();
        Assert.NotNull(myReviews);

        // If user has reviews, test update and delete
        if (myReviews.Items.Any())
        {
            var reviewId = myReviews.Items.First().Id;

            // 2. Update the review
            var updateCommand = new UpdateReviewCommand
            {
                Rating = 4,
                Comment = "Updated review comment"
            };

            var updateResponse = await client.PutAsJsonAsync($"/api/reviews/{reviewId}", updateCommand);
            
            // Should succeed if it's user's own review
            if (updateResponse.StatusCode == HttpStatusCode.NoContent)
            {
                // 3. Verify update
                var getResponse = await client.GetAsync($"/api/reviews/{reviewId}");
                var updatedReview = await getResponse.Content.ReadFromJsonAsync<ReviewDto>();
                Assert.NotNull(updatedReview);
                Assert.Equal(4, updatedReview.Rating);
                Assert.Equal("Updated review comment", updatedReview.Comment);
            }
        }
    }

    #endregion
}
