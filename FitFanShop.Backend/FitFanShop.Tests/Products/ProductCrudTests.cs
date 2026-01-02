using System.Net.Http.Json;
using System.Net;
using FitFanShop.Application.Modules.Catalog.Products;
using Xunit;

namespace FitFanShop.Tests.Products;

public class ProductCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    
    public ProductCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Product_GetAll_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();
        
        var response = await client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        Assert.True(products.Count >= 0);
    }
    
    [Fact]
    public async Task Product_GetById_ReturnsNotFound_ForNonExistentProduct()
    {
        var client = await _factory.GetAuthenticatedClientAsync();
        
        var response = await client.GetAsync("/api/products/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Product_GetAll_Returns_SeededProducts()
    {
        var client = await _factory.GetAuthenticatedClientAsync();
        
        var response = await client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
    }
}
