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
        var utcNow = DateTime.UtcNow;
        var monthStarts = Enumerable.Range(0, 6)
            .Select(offset => new DateTime(utcNow.Year, utcNow.Month, 1).AddMonths(-5 + offset))
            .ToArray();
        var periodStart = monthStarts.First();
        var periodEnd = monthStarts.Last().AddMonths(1);

        var repairCountsRaw = await dbContext.RepairRequests
            .AsNoTracking()
            .Where(r => r.RequestedDate >= periodStart && r.RequestedDate < periodEnd)
            .GroupBy(r => new { r.RequestedDate.Year, r.RequestedDate.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count()
            })
            .ToListAsync();

        var borrowCountsRaw = await dbContext.BorrowRecords
            .AsNoTracking()
            .Where(b => b.BorrowDate >= periodStart && b.BorrowDate < periodEnd)
            .GroupBy(b => new { b.BorrowDate.Year, b.BorrowDate.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count()
            })
            .ToListAsync();

        var topCategoryCounts = await dbContext.BorrowRecords
            .AsNoTracking()
            .Include(b => b.Tool)
                .ThenInclude(t => t.ToolCategory)
            .GroupBy(b => b.Tool.ToolCategory.Name)
            .Select(g => new DashboardChartPointViewModel
            {
                Label = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .ThenBy(x => x.Label)
            .Take(5)
            .ToListAsync();

        var model = new DashboardStatsViewModel
        {
            ToolsCount = await dbContext.Tools.AsNoTracking().CountAsync(),
            AvailableToolsCount = await dbContext.Tools.AsNoTracking().CountAsync(t => t.IsAvailable && t.Quantity > 0),
            RepairRequestsCount = await dbContext.RepairRequests.AsNoTracking().CountAsync(),
            PendingBorrowRecordsCount = await dbContext.BorrowRecords.AsNoTracking().CountAsync(b => b.Status == BorrowStatus.Pending || b.Status == BorrowStatus.Approved),
            ApprovedVolunteersCount = await dbContext.VolunteerProfiles.AsNoTracking().CountAsync(v => v.IsApproved),
            UpcomingSessionsCount = await dbContext.RepairSessions.AsNoTracking().CountAsync(rs => rs.StartDate >= utcNow),
            RepairedRequestsCount = await dbContext.RepairRequests.AsNoTracking().CountAsync(r => r.Status == RepairRequestStatus.Repaired),
            ActiveBorrowedToolsCount = await dbContext.BorrowRecords.AsNoTracking().CountAsync(b => b.Status == BorrowStatus.Borrowed),
            TopBorrowedToolCategories = topCategoryCounts
        };

        foreach (var monthStart in monthStarts)
        {
            var repairCount = repairCountsRaw.FirstOrDefault(x => x.Year == monthStart.Year && x.Month == monthStart.Month)?.Count ?? 0;
            var borrowCount = borrowCountsRaw.FirstOrDefault(x => x.Year == monthStart.Year && x.Month == monthStart.Month)?.Count ?? 0;

            model.RepairRequestsLastSixMonths.Add(new DashboardChartPointViewModel
            {
                Label = monthStart.ToString("MMM yyyy"),
                Value = repairCount
            });

            model.BorrowRecordsLastSixMonths.Add(new DashboardChartPointViewModel
            {
                Label = monthStart.ToString("MMM yyyy"),
                Value = borrowCount
            });
        }

        return model;
    }
}
