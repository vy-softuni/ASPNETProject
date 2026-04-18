using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.Services.Interfaces;
using RepairCircle.ViewModels.Common;
using RepairCircle.ViewModels.RepairRequests;

namespace RepairCircle.Services.Implementations;

public class RepairRequestService : IRepairRequestService
{
    private readonly ApplicationDbContext dbContext;

    public RepairRequestService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<RepairRequestIndexViewModel> GetAllAsync()
    {
        var requests = await BuildRequestListQuery()
            .OrderByDescending(r => r.RequestedDate)
            .ToListAsync();

        return new RepairRequestIndexViewModel
        {
            Requests = requests
        };
    }

    public async Task<RepairRequestIndexViewModel> GetMineAsync(string userId)
    {
        var requests = await BuildRequestListQuery()
            .Where(r => r.SubmittedByUserId == userId)
            .OrderByDescending(r => r.RequestedDate)
            .ToListAsync();

        return new RepairRequestIndexViewModel
        {
            Requests = requests
        };
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

        var repairRequest = new RepairRequest
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            ItemType = model.ItemType.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
            SubmittedByUserId = userId,
            LocationId = model.LocationId,
            RepairSessionId = model.RepairSessionId,
            RequestedDate = DateTime.UtcNow
        };

        await dbContext.RepairRequests.AddAsync(repairRequest);
        await dbContext.SaveChangesAsync();

        return repairRequest.Id;
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
                LocationName = $"{r.Location.Name} ({r.Location.City})",
                RequestedDate = r.RequestedDate
            });
    }

    private sealed class RepairRequestListProjection : RepairRequestListItemViewModel
    {
        public string SubmittedByUserId { get; set; } = string.Empty;
    }
}
