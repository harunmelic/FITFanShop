using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Media.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
{
    private readonly IAppDbContext _ctx;
    private readonly IFileStorageService _fileStorage;

    public DeleteImageCommandHandler(IAppDbContext ctx, IFileStorageService fileStorage)
    {
        _ctx = ctx;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeleteImageCommand request, CancellationToken ct)
    {
        var image = await _ctx.Images
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, ct);

        if (image == null)
            throw new FitFanShopNotFoundException($"Image with id {request.Id} not found.");

        // Delete file from disk
        await _fileStorage.DeleteFileAsync(image.FileUrl, ct);

        // Soft delete from database
        image.IsDeleted = true;
        image.ModifiedAtUtc = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(ct);
    }
}
