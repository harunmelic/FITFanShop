using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FitFanShop.Application.Modules.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IPasswordHasher<FitFanShopUserEntity> _hasher;
    private const int ResetTokenValidityMinutes = 15;

    public ResetPasswordCommandHandler(
        IAppDbContext ctx,
        IPasswordHasher<FitFanShopUserEntity> hasher)
    {
        _ctx = ctx;
        _hasher = hasher;
    }

    public async Task<ResetPasswordDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            throw new FitFanShopNotFoundException("User not found.");
        }

        // Validate reset token
        if (!ValidateResetToken(request.ResetToken, user.Id))
        {
            throw new FitFanShopBusinessRuleException("INVALID_TOKEN", "Reset token is invalid or expired.");
        }

        // Update password
        user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);
        user.TokenVersion++; // Invalidate all existing JWT tokens
        user.ModifiedAtUtc = DateTime.UtcNow;

        // Revoke all refresh tokens
        var refreshTokens = await _ctx.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var rt in refreshTokens)
        {
            rt.IsRevoked = true;
            rt.RevokedAtUtc = DateTime.UtcNow;
        }

        await _ctx.SaveChangesAsync(cancellationToken);

        return new ResetPasswordDto(true, "Password reset successfully. Please login with your new password.");
    }

    private static bool ValidateResetToken(string token, int userId)
    {
        try
        {
            var bytes = Convert.FromBase64String(token);
            var data = Encoding.UTF8.GetString(bytes);
            var parts = data.Split(':');

            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out var tokenUserId) || tokenUserId != userId)
                return false;

            if (!long.TryParse(parts[1], out var timestamp))
                return false;

            var tokenTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            var now = DateTimeOffset.UtcNow;

            return (now - tokenTime).TotalMinutes <= ResetTokenValidityMinutes;
        }
        catch
        {
            return false;
        }
    }
}
