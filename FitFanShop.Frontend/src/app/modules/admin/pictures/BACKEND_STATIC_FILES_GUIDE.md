# Backend Static File Serving Setup

## 🚨 Current Issue: 404 for Image Files

The frontend can upload images to backend, but gets **404 Not Found** when trying to access the image URLs.

## 🛠️ Backend Configuration Needed

### 1. **ASP.NET Core Static Files**

In `Program.cs` or `Startup.cs`:

```csharp
// Add this BEFORE app.UseRouting()
app.UseStaticFiles(); // Default: serves from wwwroot folder

// OR serve from custom uploads folder:
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(env.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});
```

### 2. **ImagesController Upload Method**

```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded");

    // Create uploads directory if not exists
    var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "images");
    Directory.CreateDirectory(uploadsPath);

    // Generate unique filename
    var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{file.FileName}";
    var filePath = Path.Combine(uploadsPath, fileName);

    // Save file to disk
    using var stream = new FileStream(filePath, FileMode.Create);
    await file.CopyToAsync(stream);

    // IMPORTANT: Return correct URL that matches static files config
    var fileUrl = $"/uploads/images/{fileName}";
    // OR if using wwwroot: var fileUrl = $"/images/{fileName}";

    return Ok(new
    {
        id = DateTime.Now.Ticks, // Use proper DB ID
        fileName = fileName,
        fileUrl = fileUrl,        // Must match static files serving path!
        fileSize = file.Length
    });
}
```

### 3. **Directory Structure**

```
YourBackendProject/
├── uploads/           ← Created by upload code
│   └── images/        ← Image files stored here
├── wwwroot/          ← Default static files (optional)
└── Controllers/
    └── ImagesController.cs
```

### 4. **CORS Configuration (if needed)**

```csharp
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
```

## 🧪 Test URL Generation

After implementing, test in browser:
- Backend URL: `https://localhost:7260`
- Upload creates: `/uploads/images/20260209_143022_example.jpg`
- Full URL: `https://localhost:7260/uploads/images/20260209_143022_example.jpg`

## ✅ Expected Console Output

Frontend will now show:
```
✅ API upload response: {id: 123, fileName: "...", fileUrl: "/uploads/images/..."}
🔗 Generated image URL: /uploads/images/20260209_143022_example.jpg
✅ Image URL accessible: /uploads/images/20260209_143022_example.jpg
```

## 🔧 Quick Fix

Most common issue: **Missing `app.UseStaticFiles()`** in backend startup configuration.