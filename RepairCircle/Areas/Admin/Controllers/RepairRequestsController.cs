using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Enums;
using RepairCircle.Data.Models;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class RepairRequestsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public RepairRequestsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? status, int? locationId, int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = dbContext.RepairRequests
            .AsNoTracking()
            .Include(r => r.SubmittedByUser)
            .Include(r => r.AssignedVolunteerProfile)
                .ThenInclude(v => v!.User)
            .Include(r => r.Location)
            .Include(r => r.RepairSession)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(r =>
                r.Title.ToLower().Contains(normalizedSearchTerm) ||
                r.ItemType.ToLower().Contains(normalizedSearchTerm) ||
                r.SubmittedByUser.Email!.ToLower().Contains(normalizedSearchTerm) ||
                (r.SubmittedByUser.FullName != null && r.SubmittedByUser.FullName.ToLower().Contains(normalizedSearchTerm)) ||
                r.Location.Name.ToLower().Contains(normalizedSearchTerm) ||
                r.Location.City.ToLower().Contains(normalizedSearchTerm));
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

        var requests = await query
            .OrderByDescending(r => r.RequestedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
        ViewBag.SearchTerm = searchTerm;
        ViewBag.LocationId = locationId;
        ViewBag.FilterStatuses = new SelectList(
            Enum.GetNames<RepairRequestStatus>().Select(s => new { Id = s, Name = s }),
            "Id",
            "Name",
            status);
        ViewBag.FilterLocations = new SelectList(
            await dbContext.Locations.AsNoTracking().OrderBy(l => l.Name).Select(l => new { l.Id, Name = $"{l.Name} ({l.City})" }).ToListAsync(),
            "Id",
            "Name",
            locationId);

        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await dbContext.RepairRequests
            .Include(r => r.SubmittedByUser)
            .Include(r => r.AssignedVolunteerProfile)
                .ThenInclude(v => v!.User)
            .Include(r => r.Location)
            .Include(r => r.RepairSession)
            .Include(r => r.Feedbacks)
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync(request.AssignedVolunteerProfileId, request.RepairSessionId, request.Status);
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int? assignedVolunteerProfileId, int? repairSessionId, RepairRequestStatus status)
    {
        var request = await dbContext.RepairRequests
            .Include(r => r.SubmittedByUser)
            .Include(r => r.AssignedVolunteerProfile)
                .ThenInclude(v => v!.User)
            .Include(r => r.Location)
            .Include(r => r.RepairSession)
            .Include(r => r.Feedbacks)
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (request is null)
        {
            return NotFound();
        }

        if (assignedVolunteerProfileId.HasValue)
        {
            var volunteerExists = await dbContext.VolunteerProfiles
                .AnyAsync(v => v.Id == assignedVolunteerProfileId.Value && v.IsApproved);
            if (!volunteerExists)
            {
                ModelState.AddModelError(string.Empty, "The selected volunteer is invalid or not approved.");
            }
        }

        if (repairSessionId.HasValue)
        {
            var sessionExists = await dbContext.RepairSessions
                .AnyAsync(rs => rs.Id == repairSessionId.Value);
            if (!sessionExists)
            {
                ModelState.AddModelError(string.Empty, "The selected repair session does not exist.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(assignedVolunteerProfileId, repairSessionId, status);
            request.AssignedVolunteerProfileId = assignedVolunteerProfileId;
            request.RepairSessionId = repairSessionId;
            request.Status = status;
            return View(nameof(Details), request);
        }

        request.AssignedVolunteerProfileId = assignedVolunteerProfileId;
        request.RepairSessionId = repairSessionId;
        request.Status = status;
        request.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Repair request updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task PopulateLookupsAsync(int? selectedVolunteerProfileId, int? selectedRepairSessionId, RepairRequestStatus selectedStatus)
    {
        var volunteers = await dbContext.VolunteerProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Where(v => v.IsApproved)
            .OrderBy(v => v.User.FullName)
            .Select(v => new
            {
                v.Id,
                Name = v.User.FullName ?? v.User.UserName ?? v.User.Email ?? "Unknown user"
            })
            .ToListAsync();

        var sessions = await dbContext.RepairSessions
            .AsNoTracking()
            .OrderBy(rs => rs.StartDate)
            .Select(rs => new
            {
                rs.Id,
                Name = rs.Title
            })
            .ToListAsync();

        ViewBag.Volunteers = new SelectList(volunteers, "Id", "Name", selectedVolunteerProfileId);
        ViewBag.RepairSessions = new SelectList(sessions, "Id", "Name", selectedRepairSessionId);
        ViewBag.Statuses = new SelectList(Enum.GetValues<RepairRequestStatus>().Select(s => new { Id = s, Name = s.ToString() }), "Id", "Name", selectedStatus);
    }
}
