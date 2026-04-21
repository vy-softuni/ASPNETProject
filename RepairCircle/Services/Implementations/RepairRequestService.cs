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
        return await GetPagedAsync(dbContext.RepairRequests.AsNoTracking(), searchTerm, status, locationId, page, pageSize);
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
            dbContext.RepairRequests.AsNoTracking().Where(r => r.SubmittedByUserId == userId),
            searchTerm,
            status,
            locationId,
            page,
            pageSize);
    }

    public async Task<RepairRequestDetailsViewModel?> GetByIdAsync(int id, string? currentUserId = null)
    {
        var row = await dbContext.RepairRequests
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.ItemType,
                r.ImageUrl,
                r.RequestReference,
                r.Status,
                SubmittedBy = r.SubmittedByUser.FullName ?? r.SubmittedByUser.UserName ?? r.SubmittedByUser.Email ?? "Unknown user",
                SubmittedByUserId = r.SubmittedByUserId,
                AssignedVolunteer = r.AssignedVolunteerProfile != null
                    ? (r.AssignedVolunteerProfile.User.FullName ?? r.AssignedVolunteerProfile.User.UserName ?? r.AssignedVolunteerProfile.User.Email)
                    : null,
                LocationName = r.Location.Name,
                City = r.Location.City,
                Address = r.Location.Address,
                Latitude = r.Location.Latitude,
                Longitude = r.Location.Longitude,
                RepairSessionTitle = r.RepairSession != null ? r.RepairSession.Title : null,
                r.RequestedDate,
                Feedback = r.Feedbacks
                    .OrderByDescending(f => f.CreatedOn)
                    .Select(f => new RepairRequestFeedbackViewModel
                    {
                        UserName = f.User.FullName ?? f.User.UserName ?? f.User.Email ?? "Unknown user",
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedOn = f.CreatedOn
                    })
                    .ToList(),
                HasUserFeedback = currentUserId != null && r.Feedbacks.Any(f => f.UserId == currentUserId)
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return new RepairRequestDetailsViewModel
        {
            Id = row.Id,
            Title = row.Title,
            Description = row.Description,
            ItemType = row.ItemType,
            ImageUrl = row.ImageUrl,
            RequestReference = row.RequestReference,
            Status = row.Status.ToString(),
            SubmittedBy = row.SubmittedBy,
            AssignedVolunteer = row.AssignedVolunteer,
            LocationName = row.LocationName,
            City = row.City,
            Address = row.Address,
            Latitude = row.Latitude,
            Longitude = row.Longitude,
            RepairSessionTitle = row.RepairSessionTitle,
            RequestedDate = row.RequestedDate,
            Feedback = row.Feedback,
            CanLeaveFeedback = currentUserId != null && row.SubmittedByUserId == currentUserId && !row.HasUserFeedback
        };
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
        IQueryable<RepairRequest> query,
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
            var likeTerm = $"%{searchTerm.Trim()}%";
            query = query.Where(r =>
                EF.Functions.Like(r.Title, likeTerm) ||
                EF.Functions.Like(r.ItemType, likeTerm) ||
                EF.Functions.Like(r.Location.Name, likeTerm) ||
                EF.Functions.Like(r.Location.City, likeTerm) ||
                EF.Functions.Like(r.SubmittedByUser.FullName ?? string.Empty, likeTerm) ||
                EF.Functions.Like(r.SubmittedByUser.UserName ?? string.Empty, likeTerm) ||
                EF.Functions.Like(r.SubmittedByUser.Email ?? string.Empty, likeTerm) ||
                (r.AssignedVolunteerProfile != null && (
                    EF.Functions.Like(r.AssignedVolunteerProfile.User.FullName ?? string.Empty, likeTerm) ||
                    EF.Functions.Like(r.AssignedVolunteerProfile.User.UserName ?? string.Empty, likeTerm) ||
                    EF.Functions.Like(r.AssignedVolunteerProfile.User.Email ?? string.Empty, likeTerm)
                )));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<RepairRequestStatus>(status, out var parsedStatus))
        {
            query = query.Where(r => r.Status == parsedStatus);
        }

        if (locationId.HasValue)
        {
            query = query.Where(r => r.LocationId == locationId.Value);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var requestRows = await query
            .OrderByDescending(r => r.RequestedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.ItemType,
                r.Status,
                SubmittedBy = r.SubmittedByUser.FullName ?? r.SubmittedByUser.UserName ?? r.SubmittedByUser.Email ?? "Unknown user",
                AssignedVolunteer = r.AssignedVolunteerProfile != null
                    ? (r.AssignedVolunteerProfile.User.FullName ?? r.AssignedVolunteerProfile.User.UserName ?? r.AssignedVolunteerProfile.User.Email)
                    : null,
                LocationName = r.Location.Name,
                City = r.Location.City,
                r.RequestedDate
            })
            .ToListAsync();

        var requests = requestRows
            .Select(r => new RepairRequestListItemViewModel
            {
                Id = r.Id,
                Title = r.Title,
                ItemType = r.ItemType,
                Status = r.Status.ToString(),
                SubmittedBy = r.SubmittedBy,
                AssignedVolunteer = r.AssignedVolunteer,
                LocationName = $"{r.LocationName} ({r.City})",
                RequestedDate = r.RequestedDate
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
}
