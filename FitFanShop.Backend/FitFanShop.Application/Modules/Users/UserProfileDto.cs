namespace FitFanShop.Application.Modules.Users;

public class UserProfileDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsMember { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime RegistrationDate { get; set; }
}
