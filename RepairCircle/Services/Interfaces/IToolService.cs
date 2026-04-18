using RepairCircle.ViewModels.Tools;

namespace RepairCircle.Services.Interfaces;

public interface IToolService
{
    Task<ToolIndexViewModel> GetAllAsync(string? searchTerm = null, int? categoryId = null, int? locationId = null, bool? onlyAvailable = null);
    Task<ToolDetailsViewModel?> GetByIdAsync(int id);
}
