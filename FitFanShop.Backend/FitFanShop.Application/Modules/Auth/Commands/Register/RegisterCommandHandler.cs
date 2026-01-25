using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Application.Modules.Auth.Commands.Login;
using FitFanShop.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginCommandDto>
{
    private readonly IAppDbContext _ctx;
    private readonly IPasswordHasher<FitFanShopUserEntity> _hasher;
    private readonly IJwtTokenService _jwt;

    public RegisterCommandHandler(
        IAppDbContext ctx,
        IPasswordHasher<FitFanShopUserEntity> hasher,
        IJwtTokenService jwt)
    {
        _ctx = ctx;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginCommandDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _ctx.Users
            .AnyAsync(u => u.Email.ToLower() == email, cancellationToken);

        if (emailExists)
            throw new FitFanShopBusinessRuleException("EMAIL_ALREADY_EXISTS", "User with this email already exists.");

        var user = new FitFanShopUserEntity
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            RoleId = (int)RoleType.User,
            IsEnabled = true,
            RegistrationDate = DateTime.UtcNow,
            SecurityQuestion = request.SecurityQuestion,
            SecurityAnswerHash = _hasher.HashPassword(null, request.SecurityAnswer)
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync(cancellationToken);

        var tokens = _jwt.IssueTokens(user);

        _ctx.RefreshTokens.Add(new Domain.Entities.Identity.RefreshTokenEntity
        {
            TokenHash = tokens.RefreshTokenHash,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc,
            UserId = user.Id,
            Fingerprint = string.Empty
        });

        await _ctx.SaveChangesAsync(cancellationToken);

        return new LoginCommandDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshTokenRaw,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };
    }
}

