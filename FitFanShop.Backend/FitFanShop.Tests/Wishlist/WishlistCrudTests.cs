using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Modules.Commerce.Wishlist;
using FitFanShop.Application.Modules.Commerce.Wishlist.Commands.AddToWishlist;
using Xunit;

namespace FitFanShop.Tests.Wishlist;

public class WishlistCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public WishlistCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Wishlist_GetOrCreate_ReturnsEmptyWishlist()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        var response = await client.GetAsync("/api/wishlist");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var wishlist = await response.Content.ReadFromJsonAsync<WishlistDto>();
        Assert.NotNull(wishlist);
        Assert.Empty(wishlist.Items);
        Assert.Equal(0, wishlist.ItemCount);
    }

    [Fact]
    public async Task Wishlist_AddProduct_AddsNewItem()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        var addCommand = new AddToWishlistCommand
        {
            ProductId = 1
        };

        var response = await client.PostAsJsonAsync("/api/wishlist/items", addCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var wishlist = await response.Content.ReadFromJsonAsync<WishlistDto>();
        Assert.NotNull(wishlist);
        Assert.Single(wishlist.Items);
        Assert.Equal(1, wishlist.Items[0].ProductId);
    }

    [Fact]
    public async Task Wishlist_AddRemoveAdd_SameProduct_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        var addResponse1 = await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 1 });
        Assert.Equal(HttpStatusCode.OK, addResponse1.StatusCode);

        var wishlist1 = await addResponse1.Content.ReadFromJsonAsync<WishlistDto>();
        var itemId = wishlist1!.Items[0].Id;

        var removeResponse = await client.DeleteAsync($"/api/wishlist/items/{itemId}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var addResponse2 = await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 1 });
        Assert.Equal(HttpStatusCode.OK, addResponse2.StatusCode);

        var wishlist2 = await addResponse2.Content.ReadFromJsonAsync<WishlistDto>();
        Assert.NotNull(wishlist2);
        Assert.Single(wishlist2.Items);
        Assert.Equal(1, wishlist2.Items[0].ProductId);
    }

    [Fact]
    public async Task Wishlist_CheckProduct_ReturnsTrue_IfInWishlist()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 1 });

        var checkResponse = await client.GetAsync("/api/wishlist/check/1");
        Assert.Equal(HttpStatusCode.OK, checkResponse.StatusCode);

        var checkResult = await checkResponse.Content.ReadFromJsonAsync<CheckProductInWishlistDto>();
        Assert.NotNull(checkResult);
        Assert.True(checkResult.IsInWishlist);
        Assert.NotNull(checkResult.ItemId);
    }

    [Fact]
    public async Task Wishlist_CheckProduct_ReturnsFalse_IfNotInWishlist()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        var checkResponse = await client.GetAsync("/api/wishlist/check/999");
        Assert.Equal(HttpStatusCode.OK, checkResponse.StatusCode);

        var checkResult = await checkResponse.Content.ReadFromJsonAsync<CheckProductInWishlistDto>();
        Assert.NotNull(checkResult);
        Assert.False(checkResult.IsInWishlist);
        Assert.Null(checkResult.ItemId);
    }

    [Fact]
    public async Task Wishlist_RemoveItem_DeletesItemFromWishlist()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/wishlist");

        var addResponse = await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 1 });
        var wishlist = await addResponse.Content.ReadFromJsonAsync<WishlistDto>();
        var itemId = wishlist!.Items[0].Id;

        var removeResponse = await client.DeleteAsync($"/api/wishlist/items/{itemId}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/wishlist");
        var emptyWishlist = await getResponse.Content.ReadFromJsonAsync<WishlistDto>();
        Assert.NotNull(emptyWishlist);
        Assert.Empty(emptyWishlist.Items);
    }

    [Fact]
    public async Task Wishlist_ClearWishlist_RemovesAllItems()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 1 });
        await client.PostAsJsonAsync("/api/wishlist/items", new AddToWishlistCommand { ProductId = 2 });

        var clearResponse = await client.DeleteAsync("/api/wishlist");
        Assert.Equal(HttpStatusCode.NoContent, clearResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/wishlist");
        var wishlist = await getResponse.Content.ReadFromJsonAsync<WishlistDto>();
        Assert.NotNull(wishlist);
        Assert.Empty(wishlist.Items);
    }
}
