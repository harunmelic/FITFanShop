using MediatR;

namespace FitFanShop.Application.Modules.Media.Images.Queries.GetAllImages;

public class GetAllImagesQuery : IRequest<List<ImageDto>>
{
}
