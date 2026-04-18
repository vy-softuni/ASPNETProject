using RepairCircle.ViewModels.Admin;

namespace RepairCircle.Services.Interfaces;

public interface IAdminDashboardService
{
    Task<DashboardStatsViewModel> GetStatisticsAsync();
}
