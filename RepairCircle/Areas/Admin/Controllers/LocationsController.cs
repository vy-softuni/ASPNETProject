using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class LocationsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public LocationsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var locations = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .ToListAsync();

        return View(locations);
    }

    [HttpGet]
    public IActionResult Create() => View(new Location());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Location model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await dbContext.Locations.AddAsync(model);
        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Location created successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await dbContext.Locations.FindAsync(id);
        if (location is null)
        {
            return NotFound();
        }

        return View(location);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Location model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var location = await dbContext.Locations.FindAsync(id);
        if (location is null)
        {
            return NotFound();
        }

        location.Name = model.Name;
        location.Address = model.Address;
        location.City = model.City;
        location.Description = model.Description;
        location.Latitude = model.Latitude;
        location.Longitude = model.Longitude;
        location.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Location updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await dbContext.Locations.FindAsync(id);
        if (location is not null)
        {
            dbContext.Locations.Remove(location);
            await dbContext.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Location deleted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }
}
