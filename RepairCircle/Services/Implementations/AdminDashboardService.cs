using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Admin;

namespace RepairCircle.Services.Implementations;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ApplicationDbContext dbContext;

    public AdminDashboardService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<DashboardStatsViewModel> GetStatisticsAsync()
    {
        return new DashboardStatsViewModel
        {
            ToolsCount = await dbContext.Tools.AsNoTracking().CountAsync(),
            AvailableToolsCount = await dbContext.Tools.AsNoTracking().CountAsync(t => t.IsAvailable && t.Quantity > 0),
            RepairRequestsCount = await dbContext.RepairRequests.AsNoTracking().CountAsync(),
            PendingBorrowRecordsCount = await dbContext.BorrowRecords.AsNoTracking().CountAsync(b => b.Status == BorrowStatus.Pending || b.Status == BorrowStatus.Approved),
            ApprovedVolunteersCount = await dbContext.VolunteerProfiles.AsNoTracking().CountAsync(v => v.IsApproved),
            UpcomingSessionsCount = await dbContext.RepairSessions.AsNoTracking().CountAsync(rs => rs.StartDate >= DateTime.UtcNow)
        };
    }
}
