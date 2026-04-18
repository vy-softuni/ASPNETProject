using RepairCircle.ViewModels.RepairSessions;

namespace RepairCircle.Services.Interfaces;

public interface IRepairSessionService
{
    Task<RepairSessionIndexViewModel> GetAllUpcomingAsync(int page = 1, int pageSize = 6);
    Task<RepairSessionDetailsViewModel?> GetByIdAsync(int id);
}
