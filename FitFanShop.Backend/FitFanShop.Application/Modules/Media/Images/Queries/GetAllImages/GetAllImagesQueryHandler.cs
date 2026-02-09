using FitFanShop.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Media.Images.Queries.GetAllImages;

public class GetAllImagesQueryHandler : IRequestHandler<GetAllImagesQuery, List<ImageDto>>
{
    private readonly IAppDbContext _ctx;

    public GetAllImagesQueryHandler(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<ImageDto>> Handle(GetAllImagesQuery request, CancellationToken ct)
    {
        return await _ctx.Images
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.UploadDateUtc)
            .Select(i => new ImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                FileUrl = i.FileUrl,
                FileSize = i.FileSize,
                MimeType = i.MimeType,
                UploadDateUtc = i.UploadDateUtc
            })
            .ToListAsync(ct);
    }
}
