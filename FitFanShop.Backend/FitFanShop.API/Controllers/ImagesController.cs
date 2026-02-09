using FitFanShop.Application.Modules.Media.Images;
using FitFanShop.Application.Modules.Media.Images.Commands.DeleteImage;
using FitFanShop.Application.Modules.Media.Images.Commands.UploadImage;
using FitFanShop.Application.Modules.Media.Images.Queries.GetAllImages;
using FitFanShop.Application.Modules.Media.Images.Queries.GetImageById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFanShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;

    public ImagesController(IMediator mediator, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _env = env;
    }

    /// <summary>
    /// Health check endpoint to verify upload folder configuration
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult HealthCheck()
    {
        var webRootPath = _env.WebRootPath ?? "NOT_SET";
        var contentRootPath = _env.ContentRootPath;
        var uploadsPath = Path.Combine(webRootPath, "uploads", "images");
        var exists = Directory.Exists(uploadsPath);
        
        // List files if directory exists
        var files = exists 
            ? Directory.GetFiles(uploadsPath).Select(f => Path.GetFileName(f)).ToList()
            : new List<string>();
        
        return Ok(new 
        { 
            WebRootPath = webRootPath,
            ContentRootPath = contentRootPath,
            UploadsPath = uploadsPath,
            DirectoryExists = exists,
            FilesCount = files.Count,
            SampleFiles = files.Take(5),
            StaticFilesConfig = new
            {
                RequestPath = "/uploads",
                PhysicalPath = uploadsPath
            },
            Message = exists 
                ? $"? Upload directory is configured correctly with {files.Count} files" 
                : "?? Upload directory does not exist - it will be created on first upload",
            Instructions = new
            {
                UploadEndpoint = "/api/images/upload",
                ViewImage = "/uploads/images/{filename}",
                Example = files.Any() ? $"/uploads/images/{files.First()}" : "Upload an image first"
            }
        });
    }

    /// <summary>
    /// Debug endpoint to test if a specific file is accessible
    /// </summary>
    [HttpGet("debug/test-file/{fileName}")]
    [AllowAnonymous]
    public IActionResult TestFileAccess(string fileName)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath ?? "", "uploads", "images");
        var filePath = Path.Combine(uploadsPath, fileName);
        var exists = System.IO.File.Exists(filePath);
        
        return Ok(new
        {
            FileName = fileName,
            FilePath = filePath,
            FileExists = exists,
            PublicUrl = $"/uploads/images/{fileName}",
            Message = exists 
                ? $"? File exists at {filePath}" 
                : $"? File not found at {filePath}",
            Suggestion = exists
                ? $"Try accessing: {Request.Scheme}://{Request.Host}/uploads/images/{fileName}"
                : "File does not exist. Upload it first."
        });
    }

    /// <summary>
    /// Upload a new image
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UploadImageResponseDto>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided" });

        try
        {
            using var stream = file.OpenReadStream();
            var command = new UploadImageCommand 
            { 
                FileStream = stream,
                FileName = file.FileName,
                MimeType = file.ContentType,
                FileSize = file.Length
            };
            var result = await _mediator.Send(command);
            
            // Return enhanced response with full URL and debug info
            return Ok(new 
            {
                result.Id,
                result.FileName,
                result.FileUrl,
                result.FileSize,
                FullUrl = $"{Request.Scheme}://{Request.Host}{result.FileUrl}",
                Debug = new
                {
                    UploadedAt = DateTime.UtcNow,
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                error = "Upload failed",
                message = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Get all images
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ImageDto>>> GetAll()
    {
        var query = new GetAllImagesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get image by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ImageDto>> GetById(int id)
    {
        var query = new GetImageByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Delete image by ID
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteImageCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
