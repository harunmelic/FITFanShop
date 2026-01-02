using System.ComponentModel.DataAnnotations;
namespace FitFanShop.Shared.Options;
public sealed class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";
    [Required] public string Main { get; init; } = default!;
}