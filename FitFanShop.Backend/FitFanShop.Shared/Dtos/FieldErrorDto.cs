namespace FitFanShop.Shared.Dtos;
public sealed class FieldErrorDto
{
    public string Field { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? ErrorCode { get; set; }
}
