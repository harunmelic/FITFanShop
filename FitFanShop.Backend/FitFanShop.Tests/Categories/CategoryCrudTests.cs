using System.Net.Http.Json;
using System.Net;
using FitFanShop.Application.Modules.Catalog.Categories;
using FitFanShop.Application.Modules.Catalog.Categories.Commands;
using FitFanShop.Application.Modules.Catalog.Categories.Queries;
using Xunit;
namespace FitFanShop.Tests.Categories;
public class CategoryCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public CategoryCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    [Fact]
    public async Task Category_CRUD_And_Patch_Works_For_Admin()
    {
        var client = await _factory.GetAuthenticatedClientAsync();
        var create = new CreateCategoryCommand
        {
            Name = "Test Category",
            Description = "Test Description",
            IsEnabled = true
        };
        var createResp = await client.PostAsJsonAsync("/api/categories", create);
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = await createResp.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(created);
        Assert.Equal("Test Category", created.Name);
        var getResp = await client.GetAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var get = await getResp.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(get);
        Assert.Equal(created.Id, get.Id);
        var update = new UpdateCategoryCommand
        {
            Name = "Updated Category",
            Description = "Updated Description",
            IsEnabled = false
        };
        var updateResp = await client.PutAsJsonAsync($"/api/categories/{created.Id}", update);
        Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);
        var updated = await updateResp.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Category", updated.Name);
        Assert.False(updated.IsEnabled);
        var patchDto = new SetCategoryEnabledDto { IsEnabled = true };
        var patchResp = await client.PatchAsJsonAsync($"/api/categories/{created.Id}/enabled", patchDto);
        Assert.Equal(HttpStatusCode.NoContent, patchResp.StatusCode);
        var getAfterPatchResp = await client.GetAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getAfterPatchResp.StatusCode);
        var afterPatch = await getAfterPatchResp.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(afterPatch);
        Assert.True(afterPatch.IsEnabled);
        var deleteResp = await client.DeleteAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
    }
    [Fact]
    public async Task Category_GetAll_Works()
    {
        var client = await _factory.GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.True(categories.Count >= 0);
    }
}
