using Microsoft.AspNetCore.Http;
using RepairCircle.Services.Interfaces;

namespace RepairCircle.Services.Implementations;

public class FileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private const long MaxImageSizeBytes = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    public bool TryValidateImage(IFormFile? file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file is null || file.Length == 0)
        {
            return true;
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            errorMessage = "Please upload an image file in JPG, JPEG, PNG, WEBP, or GIF format.";
            return false;
        }

        if (file.Length > MaxImageSizeBytes)
        {
            errorMessage = "The image file size must be 5 MB or less.";
            return false;
        }

        return true;
    }

    public async Task<string> SaveImageAsync(IFormFile file, string subFolder, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var safeSubFolder = (subFolder ?? string.Empty).Trim().Trim('/', '\\');
        var uploadsRoot = Path.Combine(environment.WebRootPath, "uploads", safeSubFolder);
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{safeSubFolder}/{fileName}";
    }

    public void DeleteIfLocalUpload(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var normalized = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.Combine(environment.WebRootPath, normalized);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }
}
