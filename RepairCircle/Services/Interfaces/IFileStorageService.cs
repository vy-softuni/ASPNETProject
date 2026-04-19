using Microsoft.AspNetCore.Http;

namespace RepairCircle.Services.Interfaces;

public interface IFileStorageService
{
    bool TryValidateImage(IFormFile? file, out string errorMessage);
    Task<string> SaveImageAsync(IFormFile file, string subFolder, CancellationToken cancellationToken = default);
    void DeleteIfLocalUpload(string? relativePath);
}
