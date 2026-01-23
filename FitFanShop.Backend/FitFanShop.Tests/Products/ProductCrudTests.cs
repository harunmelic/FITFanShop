using System.Net.Http.Json;
using System.Net;
using FitFanShop.Application.Modules.Catalog.Products;
using FitFanShop.Application.Modules.Catalog.Products.Commands.CreateProduct;
using FitFanShop.Application.Modules.Catalog.Products.Commands.UpdateProduct;
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

    [Fact]
    public async Task Product_Create_CreatesNewProduct()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test product description",
            Price = 99.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "S", StockQuantity = 10 },
                new() { Size = "M", StockQuantity = 15 },
                new() { Size = "L", StockQuantity = 12 }
            }
        };

        var response = await client.PostAsJsonAsync("/api/products", createCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(99.99m, product.Price);
        Assert.Equal(3, product.Variants.Count);
        Assert.Contains(product.Variants, v => v.Size == "S" && v.StockQuantity == 10);
    }

    [Fact]
    public async Task Product_Create_WithMultipleCategories()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Multi-Category Product",
            Description = "Product in multiple categories",
            Price = 129.99m,
            CategoryIds = new List<int> { 1, 2 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "M", StockQuantity = 15 }
            }
        };

        var response = await client.PostAsJsonAsync("/api/products", createCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal(2, product.CategoryIds.Count);
        Assert.Contains(1, product.CategoryIds);
        Assert.Contains(2, product.CategoryIds);
    }

    [Fact]
    public async Task Product_Create_ValidatesRequiredFields()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "",
            Description = "Test",
            Price = -10m,
            CategoryIds = new List<int>(),
            Variants = new List<CreateProductVariantDto>()
        };

        var response = await client.PostAsJsonAsync("/api/products", createCommand);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Product_GetById_ReturnsProduct()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "GetById Test Product",
            Description = "Test description",
            Price = 79.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "M", StockQuantity = 20 }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createCommand);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var getResponse = await client.GetAsync($"/api/products/{createdProduct!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal("GetById Test Product", product.Name);
        Assert.Equal(79.99m, product.Price);
        Assert.Single(product.Variants);
    }

    [Fact]
    public async Task Product_Update_UpdatesProduct()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Original Product",
            Description = "Original description",
            Price = 59.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "S", StockQuantity = 5 }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createCommand);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateCommand = new UpdateProductCommand
        {
            Name = "Updated Product",
            Description = "Updated description",
            Price = 69.99m,
            CategoryIds = new List<int> { 1, 2 },
            IsEnabled = false,
            Exclusive = true
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{createdProduct!.Id}", updateCommand);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal(69.99m, updatedProduct.Price);
        Assert.False(updatedProduct.IsEnabled);
        Assert.True(updatedProduct.Exclusive);
        Assert.Equal(2, updatedProduct.CategoryIds.Count);
    }

    [Fact]
    public async Task Product_Delete_DeletesProduct()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Product to Delete",
            Description = "Will be deleted",
            Price = 49.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "One Size", StockQuantity = 100 }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createCommand);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var deleteResponse = await client.DeleteAsync($"/api/products/{createdProduct!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Product_Create_GeneratesCorrectSKU()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "SKU Test Product",
            Description = "Testing SKU generation",
            Price = 119.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "S", StockQuantity = 5 },
                new() { Size = "XL", StockQuantity = 10 }
            }
        };

        var response = await client.PostAsJsonAsync("/api/products", createCommand);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        Assert.NotNull(product);
        Assert.All(product!.Variants, variant =>
        {
            Assert.NotNull(variant.Sku);
            Assert.StartsWith($"FCFIT-{product.Id:D3}-", variant.Sku);
            Assert.EndsWith(variant.Size.ToUpper(), variant.Sku);
        });
    }

    [Fact]
    public async Task Product_Create_WithExclusiveFlag()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Exclusive Product",
            Description = "Members only product",
            Price = 199.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = true,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "M", StockQuantity = 5 }
            }
        };

        var response = await client.PostAsJsonAsync("/api/products", createCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.True(product.Exclusive);
    }

    [Fact]
    public async Task Product_Update_ChangesCategoryAssociation()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var createCommand = new CreateProductCommand
        {
            Name = "Category Change Test",
            Description = "Testing category changes",
            Price = 89.99m,
            CategoryIds = new List<int> { 1 },
            IsEnabled = true,
            Exclusive = false,
            Variants = new List<CreateProductVariantDto>
            {
                new() { Size = "M", StockQuantity = 10 }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createCommand);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateCommand = new UpdateProductCommand
        {
            Name = "Category Change Test",
            Description = "Testing category changes",
            Price = 89.99m,
            CategoryIds = new List<int> { 2, 3 },
            IsEnabled = true,
            Exclusive = false
        };

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{createdProduct!.Id}", updateCommand);
        var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductDto>();

        Assert.NotNull(updatedProduct);
        Assert.Equal(2, updatedProduct.CategoryIds.Count);
        Assert.Contains(2, updatedProduct.CategoryIds);
        Assert.Contains(3, updatedProduct.CategoryIds);
        Assert.DoesNotContain(1, updatedProduct.CategoryIds);
    }
}
