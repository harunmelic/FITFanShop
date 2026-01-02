using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Sales.Orders;
using FitFanShop.Application.Modules.Sales.Orders.Commands.UpdateOrderStatus;
using Xunit;

namespace FitFanShop.Tests.Orders;

public class OrderCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public OrderCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Order_GetMyOrders_Returns_Paginated_Results()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/orders/my-orders?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResult = await response.Content.ReadFromJsonAsync<PageResult<OrderDto>>();
        Assert.NotNull(pageResult);
        Assert.NotNull(pageResult.Items);
        Assert.True(pageResult.PageSize == 10);
        Assert.True(pageResult.CurrentPage == 1);
    }

    [Fact]
    public async Task Order_GetMyOrders_WithDifferentPaging_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/orders/my-orders?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResult = await response.Content.ReadFromJsonAsync<PageResult<OrderDto>>();
        Assert.NotNull(pageResult);
        Assert.True(pageResult.PageSize == 5);
        Assert.True(pageResult.CurrentPage == 1);
    }

    [Fact]
    public async Task Order_GetById_ReturnsNotFound_ForNonExistentOrder()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/orders/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Order_UpdateStatus_RequiresValidStatus()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var updateDto = new UpdateOrderStatusDto { Status = "InvalidStatus" };
        var response = await client.PatchAsJsonAsync("/api/orders/1/status", updateDto);
        
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Order_Cancel_ReturnsNotFound_ForNonExistentOrder()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.PostAsync("/api/orders/99999/cancel", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Order_Delete_ReturnsNotFound_ForNonExistentOrder()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.DeleteAsync("/api/orders/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
