using Microsoft.EntityFrameworkCore;
using RepairCircle.Common;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Services.Implementations;

public class RepairRequestService : IRepairRequestService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IRealtimeNotificationService realtimeNotificationService;

    public RepairRequestService(ApplicationDbContext dbContext, IRealtimeNotificationService realtimeNotificationService)
    {
        this.dbContext = dbContext;
        this.realtimeNotificationService = realtimeNotificationService;
    }

    public async Task<RepairRequestIndexViewModel> GetAllAsync(
        string? searchTerm = null,
        string? status = null,
        int? locationId = null,
        int page = 1,
        int pageSize = 6)
    {
        return await GetPagedAsync(BuildRequestListQuery(), searchTerm, status, locationId, page, pageSize);
    }

    public async Task<RepairRequestIndexViewModel> GetMineAsync(
        string userId,
        string? searchTerm = null,
        string? status = null,
        int? locationId = null,
        int page = 1,
        int pageSize = 6)
    {
        return await GetPagedAsync(
            BuildRequestListQuery().Where(r => r.SubmittedByUserId == userId),
            searchTerm,
            status,
            locationId,
            page,
            pageSize);
    }

    public async Task<RepairRequestDetailsViewModel?> GetByIdAsync(int id)
    {
        return await dbContext.RepairRequests
            .AsNoTracking()
            .Include(r => r.SubmittedByUser)
            .Include(r => r.AssignedVolunteerProfile)
                .ThenInclude(v => v!.User)
            .Include(r => r.Location)
            .Include(r => r.RepairSession)
            .Include(r => r.Feedbacks)
                .ThenInclude(f => f.User)
            .Where(r => r.Id == id)
            .Select(r => new RepairRequestDetailsViewModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                ItemType = r.ItemType,
                ImageUrl = r.ImageUrl,
                RequestReference = r.RequestReference,
                Status = r.Status.ToString(),
                SubmittedBy = r.SubmittedByUser.FullName ?? r.SubmittedByUser.UserName ?? r.SubmittedByUser.Email ?? "Unknown user",
                AssignedVolunteer = r.AssignedVolunteerProfile != null
                    ? (r.AssignedVolunteerProfile.User.FullName ?? r.AssignedVolunteerProfile.User.UserName ?? r.AssignedVolunteerProfile.User.Email)
                    : null,
                LocationName = r.Location.Name,
                City = r.Location.City,
                RepairSessionTitle = r.RepairSession != null ? r.RepairSession.Title : null,
                RequestedDate = r.RequestedDate,
                Feedback = r.Feedbacks
                    .OrderByDescending(f => f.CreatedOn)
                    .Select(f => new RepairRequestFeedbackViewModel
                    {
                        UserName = f.User.FullName ?? f.User.UserName ?? f.User.Email ?? "Unknown user",
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedOn = f.CreatedOn
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RepairRequestCreateViewModel> GetCreateModelAsync()
    {
        var locations = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LookupViewModel
            {
                Id = l.Id,
                Name = $"{l.Name} ({l.City})"
            })
            .ToListAsync();

        var sessions = await dbContext.RepairSessions
            .AsNoTracking()
            .Where(rs => rs.StartDate >= DateTime.UtcNow)
            .OrderBy(rs => rs.StartDate)
            .Select(rs => new LookupViewModel
            {
                Id = rs.Id,
                Name = $"{rs.Title} - {rs.StartDate:dd.MM.yyyy HH:mm}"
            })
            .ToListAsync();

        return new RepairRequestCreateViewModel
        {
            Locations = locations,
            RepairSessions = sessions,
            Input = new RepairRequestCreateInputModel()
        };
    }

    public async Task<int> CreateAsync(RepairRequestCreateInputModel model, string userId)
    {
        var locationExists = await dbContext.Locations.AnyAsync(l => l.Id == model.LocationId);
        if (!locationExists)
        {
            return 0;
        }

        if (model.RepairSessionId.HasValue)
        {
            var sessionExists = await dbContext.RepairSessions.AnyAsync(rs => rs.Id == model.RepairSessionId.Value);
            if (!sessionExists)
            {
                return 0;
            }
        }

        var now = DateTime.UtcNow;

        var repairRequest = new RepairRequest
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            ItemType = model.ItemType.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
            RequestReference = ReferenceCodeGenerator.CreateRepairRequestReference(now),
            SubmittedByUserId = userId,
            LocationId = model.LocationId,
            RepairSessionId = model.RepairSessionId,
            RequestedDate = now
        };

        await dbContext.RepairRequests.AddAsync(repairRequest);
        await dbContext.SaveChangesAsync();

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        var location = await dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == model.LocationId);

        var submittedBy = user?.FullName ?? user?.UserName ?? user?.Email ?? "A user";
        var locationName = location is null ? "the selected location" : $"{location.Name} ({location.City})";

        await realtimeNotificationService.NotifyAdminNewRepairRequestAsync(repairRequest.Title, submittedBy, locationName, now);

        return repairRequest.Id;
    }

    private async Task<RepairRequestIndexViewModel> GetPagedAsync(
        IQueryable<RepairRequestListProjection> query,
        string? searchTerm,
        string? status,
        int? locationId,
        int page,
        int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 6 : pageSize;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(r =>
                r.Title.ToLower().Contains(normalizedSearchTerm) ||
                r.ItemType.ToLower().Contains(normalizedSearchTerm) ||
                r.LocationName.ToLower().Contains(normalizedSearchTerm) ||
                r.SubmittedBy.ToLower().Contains(normalizedSearchTerm) ||
                (r.AssignedVolunteer != null && r.AssignedVolunteer.ToLower().Contains(normalizedSearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<RepairRequestStatus>(status, out var parsedStatus))
        {
            var parsedStatusName = parsedStatus.ToString();
            query = query.Where(r => r.Status == parsedStatusName);
        }

        if (locationId.HasValue)
        {
            query = query.Where(r => r.LocationId == locationId.Value);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var requests = await query
            .OrderByDescending(r => r.RequestedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var locations = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LookupViewModel
            {
                Id = l.Id,
                Name = $"{l.Name} ({l.City})"
            })
            .ToListAsync();

        return new RepairRequestIndexViewModel
        {
            SearchTerm = searchTerm,
            Status = status,
            LocationId = locationId,
            Locations = locations,
            Statuses = Enum.GetNames<RepairRequestStatus>(),
            Requests = requests,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            }
        };
    }

    private IQueryable<RepairRequestListProjection> BuildRequestListQuery()
    {
        return dbContext.RepairRequests
            .AsNoTracking()
            .Include(r => r.SubmittedByUser)
            .Include(r => r.AssignedVolunteerProfile)
                .ThenInclude(v => v!.User)
            .Include(r => r.Location)
            .Select(r => new RepairRequestListProjection
            {
                Id = r.Id,
                Title = r.Title,
                ItemType = r.ItemType,
                Status = r.Status.ToString(),
                SubmittedByUserId = r.SubmittedByUserId,
                SubmittedBy = r.SubmittedByUser.FullName ?? r.SubmittedByUser.UserName ?? r.SubmittedByUser.Email ?? "Unknown user",
                AssignedVolunteer = r.AssignedVolunteerProfile != null
                    ? (r.AssignedVolunteerProfile.User.FullName ?? r.AssignedVolunteerProfile.User.UserName ?? r.AssignedVolunteerProfile.User.Email)
                    : null,
                LocationId = r.LocationId,
                LocationName = $"{r.Location.Name} ({r.Location.City})",
                RequestedDate = r.RequestedDate
            });
    }

    private sealed class RepairRequestListProjection : RepairRequestListItemViewModel
    {
        public string SubmittedByUserId { get; set; } = string.Empty;
        public int LocationId { get; set; }
    }
}
