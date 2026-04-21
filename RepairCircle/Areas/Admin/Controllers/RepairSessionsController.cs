using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.ViewModels.Common;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class RepairSessionsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public RepairSessionsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? locationId, bool? onlyUpcoming, int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = dbContext.RepairSessions
            .AsNoTracking()
            .Include(rs => rs.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(rs =>
                rs.Title.ToLower().Contains(normalizedSearchTerm) ||
                rs.Description.ToLower().Contains(normalizedSearchTerm) ||
                rs.Location.Name.ToLower().Contains(normalizedSearchTerm) ||
                rs.Location.City.ToLower().Contains(normalizedSearchTerm));
        }

        if (locationId.HasValue)
        {
            query = query.Where(rs => rs.LocationId == locationId.Value);
        }

        if (onlyUpcoming == true)
        {
            query = query.Where(rs => rs.EndDate >= DateTime.UtcNow);
        }

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var sessions = await query
            .OrderBy(rs => rs.StartDate)
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
        ViewBag.OnlyUpcoming = onlyUpcoming;
        await PopulateLocationsAsync(locationId);

        return View(sessions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateLocationsAsync();
        return View(new RepairSession
        {
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(7).AddHours(3),
            MaxParticipants = 10,
            AvailableSlots = 10
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairSession model)
    {
        ModelState.Remove(nameof(RepairSession.Location));
        ModelState.Remove(nameof(RepairSession.Volunteers));
        ModelState.Remove(nameof(RepairSession.RepairRequests));

        if (!ModelState.IsValid)
        {
            await PopulateLocationsAsync(model.LocationId);
            return View(model);
        }

        await dbContext.RepairSessions.AddAsync(model);
        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Repair session created successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var session = await dbContext.RepairSessions.FindAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        await PopulateLocationsAsync(session.LocationId);
        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RepairSession model)
    {
        ModelState.Remove(nameof(RepairSession.Location));
        ModelState.Remove(nameof(RepairSession.Volunteers));
        ModelState.Remove(nameof(RepairSession.RepairRequests));

        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLocationsAsync(model.LocationId);
            return View(model);
        }

        var session = await dbContext.RepairSessions.FindAsync(id);
        if (session is null)
        {
            return NotFound();
        }

        session.Title = model.Title;
        session.Description = model.Description;
        session.StartDate = model.StartDate;
        session.EndDate = model.EndDate;
        session.MaxParticipants = model.MaxParticipants;
        session.AvailableSlots = model.AvailableSlots;
        session.LocationId = model.LocationId;
        session.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Repair session updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var session = await dbContext.RepairSessions.FindAsync(id);
        if (session is not null)
        {
            dbContext.RepairSessions.Remove(session);
            await dbContext.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Repair session deleted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLocationsAsync(int? selectedLocationId = null)
    {
        ViewBag.Locations = new SelectList(
            await dbContext.Locations.AsNoTracking().OrderBy(l => l.Name).ToListAsync(),
            nameof(Location.Id),
            nameof(Location.Name),
            selectedLocationId);
    }
}
