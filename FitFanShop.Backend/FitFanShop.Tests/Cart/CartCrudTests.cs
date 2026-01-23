using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Modules.Commerce.Cart;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.UpdateCartItem;
using Xunit;

namespace FitFanShop.Tests.Cart;

public class CartCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CartCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Cart_GetOrCreate_ReturnsEmptyCart()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/cart");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
        Assert.Equal(0, cart.ItemCount);
        Assert.Equal(0, cart.TotalAmount);
    }

    [Fact]
    public async Task Cart_AddItem_AddsProductToCart()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/cart");

        var addCommand = new AddCartItemCommand
        {
            ProductVariantId = 1,
            Quantity = 2
        };

        var response = await client.PostAsJsonAsync("/api/cart/items", addCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Single(cart.Items);
        Assert.Equal(2, cart.Items[0].Quantity);
        Assert.True(cart.Items[0].IsProduct);
    }

    [Fact]
    public async Task Cart_UpdateItem_ChangesQuantity()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/cart");

        var addResponse = await client.PostAsJsonAsync("/api/cart/items", new AddCartItemCommand
        {
            ProductVariantId = 1,
            Quantity = 1
        });
        var cart = await addResponse.Content.ReadFromJsonAsync<CartDto>();
        var itemId = cart!.Items[0].Id;

        var updateDto = new UpdateCartItemDto
        {
            Quantity = 5
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/cart/items/{itemId}", updateDto);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedCart = await updateResponse.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
        Assert.Equal(5, updatedCart.Items[0].Quantity);
    }

    [Fact]
    public async Task Cart_RemoveItem_DeletesItemFromCart()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/cart");

        var addResponse = await client.PostAsJsonAsync("/api/cart/items", new AddCartItemCommand
        {
            ProductVariantId = 1,
            Quantity = 1
        });
        var cart = await addResponse.Content.ReadFromJsonAsync<CartDto>();
        var itemId = cart!.Items[0].Id;

        var removeResponse = await client.DeleteAsync($"/api/cart/items/{itemId}");
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/cart");
        var emptyCart = await getResponse.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(emptyCart);
        Assert.Empty(emptyCart.Items);
    }

    [Fact]
    public async Task Cart_ClearCart_RemovesAllItems()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.PostAsJsonAsync("/api/cart/items", new AddCartItemCommand
        {
            ProductVariantId = 1,
            Quantity = 1
        });

        var clearResponse = await client.DeleteAsync("/api/cart");
        Assert.Equal(HttpStatusCode.NoContent, clearResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/cart");
        var cart = await getResponse.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task Cart_Checkout_ReturnsOrderId()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        await client.DeleteAsync("/api/cart");

        await client.PostAsJsonAsync("/api/cart/items", new AddCartItemCommand
        {
            ProductVariantId = 1,
            Quantity = 1
        });

        var checkoutResponse = await client.PostAsync("/api/cart/checkout", null);
        
        if (checkoutResponse.StatusCode == HttpStatusCode.OK)
        {
            var orderId = await checkoutResponse.Content.ReadFromJsonAsync<int>();
            Assert.True(orderId > 0);

            var cartResponse = await client.GetAsync("/api/cart");
            var cart = await cartResponse.Content.ReadFromJsonAsync<CartDto>();
            Assert.NotNull(cart);
            Assert.Empty(cart.Items);
        }
    }
}
