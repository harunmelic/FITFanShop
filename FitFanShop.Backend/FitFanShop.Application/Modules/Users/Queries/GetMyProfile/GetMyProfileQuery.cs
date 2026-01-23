using FitFanShop.Application.Modules.Users;
using MediatR;

namespace FitFanShop.Application.Modules.Users.Queries.GetMyProfile;

public class GetMyProfileQuery : IRequest<UserProfileDto>
{
}
