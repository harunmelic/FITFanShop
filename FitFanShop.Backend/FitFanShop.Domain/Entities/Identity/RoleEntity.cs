using FitFanShop.Domain.Common;

namespace FitFanShop.Domain.Entities.Identity;

public enum RoleType
{
    User = 1,
    Admin = 2
}

public sealed class RoleEntity : BaseEntity
{
    public RoleType Type { get; set; } = RoleType.User;

    public bool IsAdmin => Type == RoleType.Admin;
    public bool IsUser => Type == RoleType.User;

    public ICollection<FitFanShopUserEntity> Users { get; private set; } = new List<FitFanShopUserEntity>();
}
