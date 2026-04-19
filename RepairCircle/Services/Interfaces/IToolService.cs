using RepairCircle.ViewModels.Tools;

namespace RepairCircle.Services.Interfaces;

public interface IToolService
{
    Task<ToolIndexViewModel> GetAllAsync(
        string? searchTerm = null,
        int? categoryId = null,
        int? locationId = null,
        string? condition = null,
        bool? onlyAvailable = null,
        int page = 1,
        int pageSize = 9);

    Task<ToolDetailsViewModel?> GetByIdAsync(int id, string? currentUserId = null);
}
