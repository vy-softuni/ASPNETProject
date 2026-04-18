using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;

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

    public async Task<IActionResult> Index()
    {
        var sessions = await dbContext.RepairSessions
            .AsNoTracking()
            .Include(rs => rs.Location)
            .OrderBy(rs => rs.StartDate)
            .ToListAsync();

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
