using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Discounts;
using FitFanShop.Application.Modules.Discounts.Commands.CreateDiscount;
using FitFanShop.Application.Modules.Discounts.Commands.UpdateDiscount;
using Xunit;

namespace FitFanShop.Tests.Discounts;

public class DiscountCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public DiscountCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Discount_Create_CreatesNewDiscount()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateDiscountCommand
        {
            Name = "Test Discount",
            Percentage = 25m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            MembersOnly = false,
            ProductIds = new List<int> { 1, 2 }
        };

        var response = await client.PostAsJsonAsync("/api/discounts", createCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var discount = await response.Content.ReadFromJsonAsync<DiscountDetailsDto>();
        Assert.NotNull(discount);
        Assert.Equal("Test Discount", discount.Name);
        Assert.Equal(25m, discount.Percentage);
        Assert.Equal(2, discount.Products.Count);
    }

    [Fact]
    public async Task Discount_GetAll_ReturnsDiscounts()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/discounts?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var discounts = await response.Content.ReadFromJsonAsync<PageResult<DiscountDto>>();
        Assert.NotNull(discounts);
        Assert.NotNull(discounts.Items);
    }

    [Fact]
    public async Task Discount_GetActive_ReturnsActiveDiscounts()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/discounts/active");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var discounts = await response.Content.ReadFromJsonAsync<List<DiscountDto>>();
        Assert.NotNull(discounts);
    }

    [Fact]
    public async Task Discount_GetById_ReturnsDiscount()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateDiscountCommand
        {
            Name = "GetById Test",
            Percentage = 15m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(15),
            MembersOnly = false,
            ProductIds = new List<int> { 1 }
        };

        var createResponse = await client.PostAsJsonAsync("/api/discounts", createCommand);
        var createdDiscount = await createResponse.Content.ReadFromJsonAsync<DiscountDetailsDto>();

        var getResponse = await client.GetAsync($"/api/discounts/{createdDiscount!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var discount = await getResponse.Content.ReadFromJsonAsync<DiscountDetailsDto>();
        Assert.NotNull(discount);
        Assert.Equal("GetById Test", discount.Name);
        Assert.Equal(15m, discount.Percentage);
    }

    [Fact]
    public async Task Discount_Update_UpdatesDiscount()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateDiscountCommand
        {
            Name = "Original Discount",
            Percentage = 20m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(20),
            MembersOnly = false,
            ProductIds = new List<int>()  // Prazan - 0 proizvoda
        };

        var createResponse = await client.PostAsJsonAsync("/api/discounts", createCommand);
        var createdDiscount = await createResponse.Content.ReadFromJsonAsync<DiscountDetailsDto>();

        var updateCommand = new UpdateDiscountCommand
        {
            Name = "Updated Discount",
            Percentage = 30m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(40),
            MembersOnly = true,
            ProductIds = new List<int> { 1, 2, 3 }  // 3 proizvoda
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/discounts/{createdDiscount!.Id}", updateCommand);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedDiscount = await updateResponse.Content.ReadFromJsonAsync<DiscountDetailsDto>();
        Assert.NotNull(updatedDiscount);
        Assert.Equal("Updated Discount", updatedDiscount.Name);
        Assert.Equal(30m, updatedDiscount.Percentage);
        Assert.True(updatedDiscount.MembersOnly);
        Assert.Equal(3, updatedDiscount.Products.Count);
    }

    [Fact]
    public async Task Discount_Delete_DeletesDiscount()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateDiscountCommand
        {
            Name = "To Delete",
            Percentage = 10m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            MembersOnly = false,
            ProductIds = new List<int> { 1 }
        };

        var createResponse = await client.PostAsJsonAsync("/api/discounts", createCommand);
        var createdDiscount = await createResponse.Content.ReadFromJsonAsync<DiscountDetailsDto>();

        var deleteResponse = await client.DeleteAsync($"/api/discounts/{createdDiscount!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/discounts/{createdDiscount.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Discount_Create_ValidatesPercentage()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateDiscountCommand
        {
            Name = "Invalid Discount",
            Percentage = 150m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            MembersOnly = false
        };

        var response = await client.PostAsJsonAsync("/api/discounts", createCommand);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
