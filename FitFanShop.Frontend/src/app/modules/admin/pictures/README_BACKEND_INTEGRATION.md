# Pictures Management - Backend Integration Guide

## Current Implementation
Currently using **localStorage** for image storage (base64 data).

**Limitations:**
- 5-10MB storage limit
- Images only in this browser/device
- Not shared across users
- Slow with many/large images

## Migration to Backend API

### Option 1: Keep LocalStorage (Current)
File: `pictures.component.ts` - Uses localStorage
- No backend needed
- Quick prototyping
- Client-side only

### Option 2: Switch to Backend API
File: `pictures-api.component.ts` - Uses backend endpoints
- Unlimited storage
- Images accessible from any device
- Shared across users
- Better performance

---

## Backend Implementation Required

### 1. Create Images Table (SQL)

```sql
CREATE TABLE Images (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FileName NVARCHAR(255) NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    FileSize BIGINT NOT NULL,
    MimeType NVARCHAR(100) NOT NULL,
    UploadDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

### 2. Backend C# .NET Implementation

#### Model: `Image.cs`

```csharp
public class Image
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public string MimeType { get; set; }
    public DateTime UploadDate { get; set; }
}

public class UploadImageResponse
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
}
```

#### Controller: `ImagesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public ImagesController(IWebHostEnvironment env, ApplicationDbContext context)
    {
        _env = env;
        _context = context;
    }

    // POST: api/images/upload
    [HttpPost("upload")]
    public async Task<ActionResult<UploadImageResponse>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("Invalid file type");

        // Validate file size (10MB)
        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("File too large. Maximum size is 10MB");

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save metadata to database
        var image = new Image
        {
            FileName = file.FileName,
            FileUrl = $"/uploads/images/{fileName}",
            FileSize = file.Length,
            MimeType = file.ContentType,
            UploadDate = DateTime.UtcNow
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return Ok(new UploadImageResponse
        {
            Id = image.Id,
            FileName = image.FileName,
            FileUrl = image.FileUrl,
            FileSize = image.FileSize
        });
    }

    // GET: api/images
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Image>>> GetAll()
    {
        return await _context.Images
            .OrderByDescending(i => i.UploadDate)
            .ToListAsync();
    }

    // GET: api/images/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Image>> GetById(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
            return NotFound();
        
        return image;
    }

    // DELETE: api/images/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
            return NotFound();

        // Delete file from disk
        var filePath = Path.Combine(_env.WebRootPath, image.FileUrl.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        // Delete from database
        _context.Images.Remove(image);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

#### Configure Static Files in `Program.cs`

```csharp
// Add this before app.Run()
app.UseStaticFiles(); // Serve files from wwwroot

// Optional: Serve uploaded images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});
```

---

## Frontend - Switch to API Version

### Step 1: Update Module Declaration

In `admin.module.ts`:

```typescript
import { PicturesApiComponent } from './pictures/pictures-api.component';

@NgModule({
  declarations: [
    // ... other components
    PicturesApiComponent  // Instead of PicturesComponent
  ]
})
```

### Step 2: Update Routing

In `admin-routing.module.ts`:

```typescript
import { PicturesApiComponent } from './pictures/pictures-api.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      // ... other routes
      { path: 'pictures', component: PicturesApiComponent }
    ]
  }
];
```

### Step 3: Update Component Selector (if needed)

If you're using the component selector elsewhere, update it from:
```html
<app-pictures></app-pictures>
```
to:
```html
<app-pictures-api></app-pictures-api>
```

---

## Cloud Storage Alternative (Azure Blob / AWS S3)

Instead of saving to local disk, you can use cloud storage:

### Azure Blob Storage Example

```csharp
// Install NuGet: Azure.Storage.Blobs

private readonly BlobServiceClient _blobServiceClient;

public ImagesController(BlobServiceClient blobServiceClient)
{
    _blobServiceClient = blobServiceClient;
}

[HttpPost("upload")]
public async Task<ActionResult<UploadImageResponse>> Upload(IFormFile file)
{
    var containerClient = _blobServiceClient.GetBlobContainerClient("product-images");
    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var blobClient = containerClient.GetBlobClient(fileName);

    await blobClient.UploadAsync(file.OpenReadStream(), true);

    var image = new Image
    {
        FileName = file.FileName,
        FileUrl = blobClient.Uri.ToString(),
        FileSize = file.Length,
        MimeType = file.ContentType,
        UploadDate = DateTime.UtcNow
    };

    _context.Images.Add(image);
    await _context.SaveChangesAsync();

    return Ok(new UploadImageResponse
    {
        Id = image.Id,
        FileName = image.FileName,
        FileUrl = image.FileUrl,
        FileSize = image.FileSize
    });
}
```

---

## Testing

1. **Start backend** with image endpoints
2. **Update environment.ts** with correct API URL
3. **Run frontend** and navigate to Pictures page
4. **Upload images** - they'll be saved to server
5. **Refresh page** - images load from database
6. **Open in another browser** - same images appear

---

## Security Considerations

1. **Authentication** - Require login to upload/delete
2. **Authorization** - Only admins can manage images
3. **File validation** - Check file signatures, not just extensions
4. **Size limits** - Enforce max file size
5. **Sanitize filenames** - Prevent path traversal attacks
6. **Rate limiting** - Prevent abuse
7. **Virus scanning** - For production environments

---

## Performance Optimization

1. **Image resizing** - Generate thumbnails on upload
2. **CDN** - Serve images from CDN
3. **Lazy loading** - Load images as user scrolls
4. **Compression** - Compress images before upload
5. **Caching** - Cache image metadata

---

## Files Overview

```
Frontend:
├── api-services/images/
│   ├── image-api.model.ts       # TypeScript models
│   └── image-api.service.ts     # HTTP service
├── modules/admin/pictures/
│   ├── pictures.component.ts          # LocalStorage version (current)
│   ├── pictures-api.component.ts      # Backend API version (new)
│   ├── pictures.component.html        # Shared template
│   └── pictures.component.scss        # Shared styles

Backend (to be created):
├── Controllers/
│   └── ImagesController.cs      # API endpoints
├── Models/
│   └── Image.cs                 # Database model
├── wwwroot/uploads/images/      # Uploaded files storage
└── Program.cs                   # Static files config
```
