using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public string TokenHash { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public FitFanShopUserEntity User { get; set; } = default!;
    public string? Fingerprint { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}