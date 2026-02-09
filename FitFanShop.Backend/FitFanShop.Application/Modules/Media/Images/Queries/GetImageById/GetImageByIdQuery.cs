using MediatR;

namespace FitFanShop.Application.Modules.Media.Images.Queries.GetImageById;

public record GetImageByIdQuery(int Id) : IRequest<ImageDto>;
