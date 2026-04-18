using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;
using RepairCircle.ViewModels.Common;

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

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10;
        page = page < 1 ? 1 : page;

        var query = dbContext.Locations.AsNoTracking();

        var totalItems = await query.CountAsync();
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);
        page = Math.Min(page, totalPages);

        var locations = await query
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Pagination = new PaginationViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

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
