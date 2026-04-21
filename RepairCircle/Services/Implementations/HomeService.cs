using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Home;
using RepairCircle.ViewModels.Tools;

namespace RepairCircle.Services.Implementations;

public class HomeService : IHomeService
{
    private readonly ApplicationDbContext dbContext;

    public HomeService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<HomeIndexViewModel> GetHomePageDataAsync()
    {
        var availableToolsCount = await dbContext.Tools
            .AsNoTracking()
            .CountAsync(t => t.IsAvailable && t.Quantity > 0);

        var activeRepairRequestsCount = await dbContext.RepairRequests
            .AsNoTracking()
            .CountAsync(r => r.Status != RepairRequestStatus.Repaired && r.Status != RepairRequestStatus.Cancelled);

        var upcomingSessionsCount = await dbContext.RepairSessions
            .AsNoTracking()
            .CountAsync(rs => rs.StartDate >= DateTime.UtcNow);

        var approvedVolunteersCount = await dbContext.VolunteerProfiles
            .AsNoTracking()
            .CountAsync(v => v.IsApproved);

        var latestAnnouncements = await dbContext.Announcements
            .AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.CreatedOn)
            .Take(3)
            .Select(a => new HomeAnnouncementViewModel
            {
                Title = a.Title,
                Content = a.Content,
                IsImportant = a.IsImportant,
                CreatedOn = a.CreatedOn
            })
            .ToListAsync();

        var featuredToolRows = await dbContext.Tools
            .AsNoTracking()
            .Where(t => t.IsAvailable && t.Quantity > 0)
            .OrderByDescending(t => t.Quantity)
            .ThenBy(t => t.Name)
            .Take(3)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.ImageUrl,
                t.Condition,
                t.IsAvailable,
                t.Quantity,
                CategoryName = t.ToolCategory.Name,
                LocationName = t.Location.Name,
                City = t.Location.City
            })
            .ToListAsync();

        var featuredTools = featuredToolRows
            .Select(t => new ToolListItemViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                Condition = t.Condition.ToString(),
                IsAvailable = t.IsAvailable,
                Quantity = t.Quantity,
                CategoryName = t.CategoryName,
                LocationName = t.LocationName,
                City = t.City
            })
            .ToList();

        return new HomeIndexViewModel
        {
            AvailableToolsCount = availableToolsCount,
            ActiveRepairRequestsCount = activeRepairRequestsCount,
            UpcomingSessionsCount = upcomingSessionsCount,
            ApprovedVolunteersCount = approvedVolunteersCount,
            LatestAnnouncements = latestAnnouncements,
            FeaturedTools = featuredTools
        };
    }
}
