using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FitFanShop.Application.Modules.Auth.Commands.VerifySecurityAnswer;

public class VerifySecurityAnswerCommandHandler : IRequestHandler<VerifySecurityAnswerCommand, VerifySecurityAnswerDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IPasswordHasher<FitFanShopUserEntity> _hasher;

    public VerifySecurityAnswerCommandHandler(
        IAppDbContext ctx,
        IPasswordHasher<FitFanShopUserEntity> hasher)
    {
        _ctx = ctx;
        _hasher = hasher;
    }

    public async Task<VerifySecurityAnswerDto> Handle(VerifySecurityAnswerCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email && !u.IsDeleted, cancellationToken);

        if (user == null || string.IsNullOrEmpty(user.SecurityAnswerHash))
        {
            throw new FitFanShopNotFoundException("User not found or security answer not set.");
        }

        var result = _hasher.VerifyHashedPassword(null, user.SecurityAnswerHash, request.SecurityAnswer);

        if (result == PasswordVerificationResult.Failed)
        {
            return new VerifySecurityAnswerDto(false, null);
        }

        // Generate a temporary reset token (valid for 15 minutes)
        var resetToken = GenerateResetToken(user.Id);

        return new VerifySecurityAnswerDto(true, resetToken);
    }

    private static string GenerateResetToken(int userId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var data = $"{userId}:{timestamp}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = Convert.ToBase64String(bytes);
        return hash;
    }
}
