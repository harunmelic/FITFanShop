using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PageResult<UserDto>>
{
    private readonly IAppDbContext _ctx;

    public GetAllUsersQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<PageResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _ctx.Users
            .Include(u => u.Role)
            .Include(u => u.MemberProfile)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.RegistrationDate)
            .Skip((request.Paging.Page - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role!.Type.ToString(),
                IsEnabled = u.IsEnabled,
                IsMember = u.IsMember,
                RegistrationDate = u.RegistrationDate
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)request.Paging.PageSize);

        return new PageResult<UserDto>
        {
            Items = users,
            TotalItems = total,
            CurrentPage = request.Paging.Page,
            PageSize = request.Paging.PageSize,
            IncludedTotal = true,
            TotalPages = totalPages
        };
    }
}
