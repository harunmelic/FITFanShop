namespace FitFanShop.Shared.Dtos;
public sealed class ErrorDto
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? TraceId { get; set; }
    public string? Details { get; set; }
}
