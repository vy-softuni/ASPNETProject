using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.RepairSessions;

namespace RepairCircle.Services.Implementations;

public class RepairSessionService : IRepairSessionService
{
    private readonly ApplicationDbContext dbContext;

    public RepairSessionService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<RepairSessionIndexViewModel> GetAllUpcomingAsync(
        string? searchTerm = null,
        int? locationId = null,
        int page = 1,
        int pageSize = 6)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 6 : pageSize;

        var sessionsData = await dbContext.RepairSessions
            .AsNoTracking()
            .Include(rs => rs.Location)
            .Where(rs => rs.EndDate >= DateTime.UtcNow)
            .ToListAsync();

        IEnumerable<RepairCircle.Data.Models.RepairSession> filteredSessions = sessionsData;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            filteredSessions = filteredSessions.Where(rs =>
                rs.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                rs.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                rs.Location.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                rs.Location.City.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (locationId.HasValue)
        {
            filteredSessions = filteredSessions.Where(rs => rs.LocationId == locationId.Value);
        }

        var totalItems = filteredSessions.Count();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var sessions = filteredSessions
            .OrderBy(rs => rs.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToList();

        var locations = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LookupViewModel
            {
                Id = l.Id,
                Name = $"{l.Name} ({l.City})"
            })
            .ToListAsync();

        return new RepairSessionIndexViewModel
        {
            SearchTerm = searchTerm,
            LocationId = locationId,
            Locations = locations,
            Sessions = sessions,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
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
                Latitude = rs.Location.Latitude,
                Longitude = rs.Location.Longitude,
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
