using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepairCircle.Data;
using RepairCircle.Data.Models;

namespace RepairCircle.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class AnnouncementsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public AnnouncementsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var announcements = await dbContext.Announcements
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();

        return View(announcements);
    }

    [HttpGet]
    public IActionResult Create() => View(new Announcement { IsPublished = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await dbContext.Announcements.AddAsync(model);
        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Announcement created successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Announcement model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        announcement.Title = model.Title;
        announcement.Content = model.Content;
        announcement.IsImportant = model.IsImportant;
        announcement.IsPublished = model.IsPublished;
        announcement.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Announcement updated successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await dbContext.Announcements.FindAsync(id);
        if (announcement is not null)
        {
            dbContext.Announcements.Remove(announcement);
            await dbContext.SaveChangesAsync();
        }

        TempData["StatusMessage"] = "Announcement deleted successfully.";
        TempData["StatusType"] = "success";
        return RedirectToAction(nameof(Index));
    }
}
