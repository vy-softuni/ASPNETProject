using RepairCircle.ViewModels.RepairSessions;

namespace RepairCircle.Services.Interfaces;

public interface IRepairSessionService
{
    Task<RepairSessionIndexViewModel> GetAllUpcomingAsync(
        string? searchTerm = null,
        int? locationId = null,
        int page = 1,
        int pageSize = 6);

    Task<RepairSessionDetailsViewModel?> GetByIdAsync(int id);
}
