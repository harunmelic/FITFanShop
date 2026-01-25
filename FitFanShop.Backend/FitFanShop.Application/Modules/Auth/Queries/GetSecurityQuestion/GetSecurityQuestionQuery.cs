using MediatR;

namespace FitFanShop.Application.Modules.Auth.Queries.GetSecurityQuestion;

public record GetSecurityQuestionQuery(string Email) : IRequest<GetSecurityQuestionDto>;

public record GetSecurityQuestionDto(string SecurityQuestion);
