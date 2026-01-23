using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Modules.Commerce.Cart.Commands.AddCartItem;
using FitFanShop.Application.Modules.Payments.Commands.MockCheckout;
using Xunit;

namespace FitFanShop.Tests.Payments;

public class PaymentCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PaymentCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Payment_MockCheckout_EmptyCart_Fails()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        // Clear cart
        await client.DeleteAsync("/api/cart");

        // Try to checkout with empty cart
        var response = await client.PostAsync("/api/payments/checkout", null);
        
        // Should fail because cart is empty
        Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
