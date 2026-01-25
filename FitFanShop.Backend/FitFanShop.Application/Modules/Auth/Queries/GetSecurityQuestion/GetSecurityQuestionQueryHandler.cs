using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Auth.Queries.GetSecurityQuestion;

public class GetSecurityQuestionQueryHandler : IRequestHandler<GetSecurityQuestionQuery, GetSecurityQuestionDto>
{
    private readonly IAppDbContext _ctx;

    public GetSecurityQuestionQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<GetSecurityQuestionDto> Handle(GetSecurityQuestionQuery request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _ctx.Users
            .Where(u => u.Email.ToLower() == email)
            .Select(u => new { u.SecurityQuestion })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null || string.IsNullOrEmpty(user.SecurityQuestion))
        {
            throw new FitFanShopBusinessRuleException("USER_NOT_FOUND", "User not found or security question not set.");
        }

        return new GetSecurityQuestionDto(user.SecurityQuestion);
    }
}
