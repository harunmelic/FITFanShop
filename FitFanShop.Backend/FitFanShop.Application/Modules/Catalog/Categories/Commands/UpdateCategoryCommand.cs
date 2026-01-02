using MediatR;
using System.Text.Json.Serialization;

namespace FitFanShop.Application.Modules.Catalog.Categories.Commands;

public class UpdateCategoryCommand : IRequest<CategoryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; } = true;
}
