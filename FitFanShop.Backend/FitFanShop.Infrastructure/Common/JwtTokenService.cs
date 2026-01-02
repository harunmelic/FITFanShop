using FitFanShop.Application.Abstractions;
using FitFanShop.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace FitFanShop.Infrastructure.Common;
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwt;
    private readonly TimeProvider _time;
    public JwtTokenService(IOptions<JwtOptions> options, TimeProvider time)
    {
        _jwt = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _time = time ?? throw new ArgumentNullException(nameof(time));
    }
    public JwtTokenPair IssueTokens(FitFanShopUserEntity user)
    {
        var nowInstant = _time.GetUtcNow();
        var nowUtc = nowInstant.UtcDateTime;
        var accessExpires = nowInstant.AddMinutes(_jwt.AccessTokenMinutes).UtcDateTime;
        var refreshExpires = nowInstant.AddDays(_jwt.RefreshTokenDays).UtcDateTime;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier,   user.Id.ToString()),
            new(ClaimTypes.Email,            user.Email),
            new("is_admin",    user.IsAdmin.ToString().ToLowerInvariant()),
            new("is_member",   user.IsMember.ToString().ToLowerInvariant()),
            new("ver",         user.TokenVersion.ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixTimeSeconds(nowInstant).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Aud, _jwt.Audience)
        };
        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: accessExpires,
            signingCredentials: creds
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        var refreshRaw = GenerateRefreshTokenRaw(64); 
        var refreshHash = HashRefreshToken(refreshRaw); 
        return new JwtTokenPair
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenRaw = refreshRaw,
            RefreshTokenHash = refreshHash,
            RefreshTokenExpiresAtUtc = refreshExpires
        };
    }
    public string HashRefreshToken(string rawToken)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        return Base64UrlEncoder.Encode(bytes);
    }
    private static string GenerateRefreshTokenRaw(int numBytes)
    {
        var bytes = RandomNumberGenerator.GetBytes(numBytes);
        return Base64UrlEncoder.Encode(bytes);
    }
    private static long ToUnixTimeSeconds(DateTimeOffset dto) => dto.ToUnixTimeSeconds();
}