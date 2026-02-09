using MediatR;

namespace FitFanShop.Application.Modules.Media.Images.Commands.DeleteImage;

public record DeleteImageCommand(int Id) : IRequest;
