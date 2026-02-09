using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Media.Images.Queries.GetImageById;

public class GetImageByIdQueryHandler : IRequestHandler<GetImageByIdQuery, ImageDto>
{
    private readonly IAppDbContext _ctx;

    public GetImageByIdQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ImageDto> Handle(GetImageByIdQuery request, CancellationToken ct)
    {
        var image = await _ctx.Images
            .Where(i => i.Id == request.Id && !i.IsDeleted)
            .Select(i => new ImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                FileUrl = i.FileUrl,
                FileSize = i.FileSize,
                MimeType = i.MimeType,
                UploadDateUtc = i.UploadDateUtc
            })
            .FirstOrDefaultAsync(ct);

        if (image == null)
            throw new FitFanShopNotFoundException($"Image with id {request.Id} not found.");

        return image;
    }
}
