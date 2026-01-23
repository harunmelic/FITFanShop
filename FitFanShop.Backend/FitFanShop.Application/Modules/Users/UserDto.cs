namespace FitFanShop.Application.Modules.Users;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool IsMember { get; set; }
    public DateTime RegistrationDate { get; set; }
}
