using RepairCircle.ViewModels.RepairSessions;

namespace RepairCircle.Services.Interfaces;

public interface IRepairSessionService
{
    Task<IReadOnlyCollection<RepairSessionListItemViewModel>> GetAllUpcomingAsync();
    Task<RepairSessionDetailsViewModel?> GetByIdAsync(int id);
}
