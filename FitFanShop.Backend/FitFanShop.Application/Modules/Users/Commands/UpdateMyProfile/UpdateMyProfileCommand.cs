using FitFanShop.Application.Modules.Users;
using MediatR;

namespace FitFanShop.Application.Modules.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommand : IRequest<UserProfileDto>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
