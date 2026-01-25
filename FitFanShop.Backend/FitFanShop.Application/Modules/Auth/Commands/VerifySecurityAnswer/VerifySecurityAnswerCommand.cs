using MediatR;

namespace FitFanShop.Application.Modules.Auth.Commands.VerifySecurityAnswer;

public record VerifySecurityAnswerCommand : IRequest<VerifySecurityAnswerDto>
{
    public required string Email { get; init; }
    public required string SecurityAnswer { get; init; }
}

public record VerifySecurityAnswerDto(bool IsValid, string? ResetToken);
