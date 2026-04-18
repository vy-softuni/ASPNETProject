using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.RepairSessions;

namespace RepairCircle.Services.Implementations;

public class RepairSessionService : IRepairSessionService
{
    private readonly ApplicationDbContext dbContext;

    public RepairSessionService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<RepairSessionListItemViewModel>> GetAllUpcomingAsync()
    {
        return await dbContext.RepairSessions
            .AsNoTracking()
            .Include(rs => rs.Location)
            .Where(rs => rs.EndDate >= DateTime.UtcNow)
            .OrderBy(rs => rs.StartDate)
            .Select(rs => new RepairSessionListItemViewModel
            {
                Id = rs.Id,
                Title = rs.Title,
                Description = rs.Description,
                StartDate = rs.StartDate,
                EndDate = rs.EndDate,
                AvailableSlots = rs.AvailableSlots,
                MaxParticipants = rs.MaxParticipants,
                LocationName = rs.Location.Name,
                City = rs.Location.City
            })
            .ToListAsync();
    }

    public async Task<RepairSessionDetailsViewModel?> GetByIdAsync(int id)
    {
        return await dbContext.RepairSessions
            .AsNoTracking()
            .Include(rs => rs.Location)
            .Include(rs => rs.Volunteers)
                .ThenInclude(v => v.User)
            .Include(rs => rs.RepairRequests)
            .Where(rs => rs.Id == id)
            .Select(rs => new RepairSessionDetailsViewModel
            {
                Id = rs.Id,
                Title = rs.Title,
                Description = rs.Description,
                StartDate = rs.StartDate,
                EndDate = rs.EndDate,
                AvailableSlots = rs.AvailableSlots,
                MaxParticipants = rs.MaxParticipants,
                LocationName = rs.Location.Name,
                Address = rs.Location.Address,
                City = rs.Location.City,
                VolunteerNames = rs.Volunteers
                    .OrderBy(v => v.User.FullName)
                    .Select(v => v.User.FullName ?? v.User.UserName ?? v.User.Email ?? "Unknown user")
                    .ToList(),
                RepairRequestTitles = rs.RepairRequests
                    .OrderBy(r => r.Title)
                    .Select(r => r.Title)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}
